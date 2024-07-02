﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.Controllers;

namespace VacationAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminVacationController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        //private readonly BlobServiceClient _blobServiceClient;
        //private readonly string _containerName;
        private readonly ILogger<AdminH1bcontroller> _logger;

        public AdminVacationController(EmpPortalContext context, ILogger<AdminH1bcontroller> logger)
        {
            _context = context;
            _logger = logger;
            //  _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzConnectionString"));
            //_containerName = configuration.GetConnectionString("AzContainerName");
        }

        // GET: api/VacationAppItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VacationAppItem>>> GetVacationAppItems()
        {
            return await _context.VacationAppItems
                .Where(entry => entry.ApprovalStatus == "Pending")
                .ToListAsync();
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


        [HttpPut("{id}/updateStatus")]
        public async Task<IActionResult> UpdateVacationStatus(long id, [FromQuery] string status)
        {
            var vacationAppItem = await _context.VacationAppItems.FindAsync(id);
            if (vacationAppItem == null)
            {
                return NotFound();
            }

            vacationAppItem.ApprovalStatus = status;
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

            return Ok(new { message = "Status updated successfully!" });
        }




        private bool VacationAppItemExists(long id)
        {
            return _context.VacationAppItems.Any(e => e.Id == id);
        }
    }
}