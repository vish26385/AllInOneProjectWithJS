using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AllInOneProject.Repositories
{
    public class PartyRepository : IPartyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public PartyRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _context = context;
        }

        public async Task<PartyMaster?> GetPartyByIdAsync(int id)
        {
            return await _context.PartyMasters.FindAsync(id);
        }

        public async Task<List<PartyMaster>> GetAllPartiesAsync()
        {
            var parties = new List<PartyMaster>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetAllParties", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                parties.Add(new PartyMaster
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? ""
                });
            }
            
            return parties;
        }

        public async Task<int> SavePartyAsync(PartyMasterRequest request)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_insertParty", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PartyName", request.PartyMaster.Name);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();            
        }

        public async Task<int> UpdatePartyAsync(PartyMasterRequest request)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_updateParty", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", request.PartyMaster.Id);
            cmd.Parameters.AddWithValue("@PartyName", request.PartyMaster.Name);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeletePartyAsync(int Id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_deleteParty", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", Id);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
