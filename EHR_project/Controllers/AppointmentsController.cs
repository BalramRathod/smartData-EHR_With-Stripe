using Dapper;
using EHR_project.Data;
using EHR_project.Dto;
using EHR_project.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EHR_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly DBContext _context;
        private DapperContext _Connectionstring;

        public AppointmentsController(DBContext context, DapperContext connectionstring)
        {
            _context = context;
            _Connectionstring = connectionstring;
        }


        [HttpGet("GetAppointmentUserId/{id}")]
        public async Task<ActionResult> GetAppointments(int id)
        {
            var DbUser = await _context.users.FindAsync(id);
            if (DbUser.User_type == 1)
            {
                var DbProvider = await _context.provider.FirstOrDefaultAsync(x => x.UserId == id);

                using var connection = _context.Database.GetDbConnection();
                var dbParams = new
                {
                    ProviderId = DbProvider.ProviderId,
                };
                var appointmentEnumarables = await connection.QueryAsync<AppointmentListPatientDto>
                (
                    "spSD_Get_Appointments_By_Provider",
                    dbParams,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var appointmentListDto = appointmentEnumarables.AsList();

                return Ok(appointmentListDto);
            }
            if (DbUser.User_type == 2)
            {
                var DbPatient = await _context.patient.FirstOrDefaultAsync(x => x.UserId == DbUser.UserId);

                using var connection = _context.Database.GetDbConnection();
                var dbParams = new
                {
                    PatientId = DbPatient.PatientId,
                };
                var appointmentEnumarables = await connection.QueryAsync<AppointmentListProviderDto>
                (
                    "spSD_Get_Appointments_By_Patient",
                    dbParams,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var appointmentListDto = appointmentEnumarables.AsList();

                return Ok(appointmentListDto);
            }

            return Ok(new
            {
                message = "Not Found"
            });
        }

        [HttpGet("GetUserDetails/{id}")]
        public async Task<ActionResult> GetUserDetails(int id)
        {
            var connection = _Connectionstring.CreateConnection();
            var query = "select *from users Where UserId=@Id";
            var dbCartDetails = await connection.QueryAsync(query, new { Id = id });
            return Ok(dbCartDetails);
        }
        [HttpGet("GetCharge/{id}")]
        public async Task<ActionResult> GetCharge(int id)
        {
            var connection = _Connectionstring.CreateConnection();
            var query = "select *from transactionHistory Where Appointment_Id=@Id";
            var dbCartDetails = await connection.QueryAsync(query, new { Id = id });
            return Ok(dbCartDetails);
        }



        [HttpPost]
        public async Task<ActionResult> PostAppointment(AppointmentDto appointmentDto)
        {
            if (_context.appointment == null)
            {
                return Problem("Entity set 'DBContext.appointment'  is null.");
            }

            Appointment appointment = appointmentDto.Adapt<Appointment>();
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();

            TransactionHistory transaction = new TransactionHistory();
            transaction.Appointment_Id = appointment.AppointmentId;
            transaction.Charge_Id = appointmentDto.Charge_Id;
            transaction.Status = "Paid";
            

            await _context.transactionHistory.AddAsync(transaction);
            await _context.SaveChangesAsync();




            return Ok(new { Message = "done" });
        }

        private bool AppointmentExists(int id)
        {
            return (_context.appointment?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        }
    }
}
