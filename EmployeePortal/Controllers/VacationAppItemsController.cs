using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.Model;



namespace VacationAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationAppItemsController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public VacationAppItemsController(EmpPortalContext context, IConfiguration configuration)
        {
            _context = context;
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzConnectionString"));
            _containerName = configuration.GetConnectionString("AzContainerName");
        }

        // GET: api/VacationAppItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacationAppItem>>> GetVacationAppItems()
        {
            return await _context.VacationAppItems.ToListAsync();
        }

        // GET: api/VacationAppItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VacationAppItem>> GetVacationAppItem(long id)
        {
            var vacationAppItem = await _context.VacationAppItems.FindAsync(id);

            if (vacationAppItem == null)
            {
                return NotFound();
            }

            return vacationAppItem;
        }

        // GET: api/VacationAppItems/5
        [HttpGet("authid/{authid}")]
        public async Task<ActionResult<List<VacationAppItem>>> GetVacationAppItemEmail(string authid)
        {
            List<VacationAppItem> vacationAppItem = await _context.VacationAppItems.Where(x => x.AuthId.Contains(authid)).ToListAsync();

            if (vacationAppItem == null)
            {
                return NotFound();
            }


            return Ok(vacationAppItem);
        }

        // PUT: api/VacationAppItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacationAppItem(long id, VacationAppItem vacationAppItem)
        {
            if (id != vacationAppItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(vacationAppItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacationAppItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/VacationAppItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VacationAppItem>> PostVacationAppItem([FromForm] IFormFile file, [FromForm] string name, [FromForm] string authid, [FromForm] string email,
            [FromForm] string Endhours, [FromForm] string Starthours, [FromForm] bool agree, [FromForm] string body, [FromForm] DateOnly vacationStartdate,
            [FromForm] DateOnly vacationEnddate)
        {
            // Console.WriteLine("this os from frontemd file:");
            // Console.WriteLine(file);
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            var vacationAppItem = new VacationAppItem
            {
                AuthId = authid,
                Name = name,
                Email = email,
                Body = body,
                Starthours = Starthours,
                Endhours = Endhours,
                VacationStartdate = vacationStartdate,
                VacationEnddate = vacationEnddate,
                //agree = agree,
                ImageUrl = blobClient.Uri.ToString(),

            };
            Console.WriteLine("this is json to store data to db");
            Console.WriteLine(vacationAppItem);
            _context.VacationAppItems.Add(vacationAppItem);
            await _context.SaveChangesAsync();

            Console.WriteLine("this is o=done to save DB");

            return CreatedAtAction(nameof(GetVacationAppItem), new { id = vacationAppItem.Id }, vacationAppItem);
        }

        // DELETE: api/VacationAppItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacationAppItem(long id)
        {
            var vacationAppItem = await _context.VacationAppItems.FindAsync(id);
            if (vacationAppItem == null)
            {
                return NotFound();
            }

            _context.VacationAppItems.Remove(vacationAppItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VacationAppItemExists(long id)
        {
            return _context.VacationAppItems.Any(e => e.Id == id);
        }
    }
}
