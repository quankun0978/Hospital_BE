using Microsoft.AspNetCore.Mvc;
using Hospital_BE.DAL.Models;
using System;
using System.Linq;
using Hospital_BE.DAL.Context;
using Hospital_BE.PL.Controllers.Base;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.Middleware;

namespace Hospital_BE.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientRecordController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public PatientRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /PatientRecord/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetByUserId(Guid userId)
        {
            var records = _context.PatientRecords.Where(x => x.UserId == userId && (x.IsActive == null || x.IsActive == true)).ToList();
            if (records == null || records.Count == 0)
                return NotFound();
            return Ok(records);
        }

        // POST: api/PatientRecord
        [HttpPost]
        public IActionResult Create([FromBody] PatientRecordDto dto)
        {
            if (dto == null) return BadRequest();
            var model = new PatientRecord
            {
                PatientId = Guid.NewGuid(),
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                HealthInsuranceNumber = dto.HealthInsuranceNumber,
                UserId = dto.UserId,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            _context.PatientRecords.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }

        // PUT: api/PatientRecord/{id}
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] PatientRecordDto dto)
        {
            var record = _context.PatientRecords.FirstOrDefault(x => x.PatientId == id && (x.IsActive == null || x.IsActive == true));
            if (record == null) return NotFound();
            // Cập nhật các trường cần thiết
            record.FullName = dto.FullName;
            record.Phone = dto.Phone;
            record.DateOfBirth = dto.DateOfBirth;
            record.Gender = dto.Gender;
            record.Address = dto.Address;
            record.Email = dto.Email;
            record.HealthInsuranceNumber = dto.HealthInsuranceNumber;
            record.UserId = dto.UserId;
            record.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok(record);
        }

        // DELETE: api/PatientRecord/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var record = _context.PatientRecords.FirstOrDefault(x => x.PatientId == id && (x.IsActive == null || x.IsActive == true));
            if (record == null) return NotFound();
            record.IsActive = false;
            record.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }
    }
}
