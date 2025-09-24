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
        public PartyRepository(string connectionString, ApplicationDbContext context)
        {
            _connectionString = connectionString;
            _context = context;
        }

        public async Task<PartyMaster?> GetPartyByIdAsync(int id)
        {
            return await _context.PartyMasters.FindAsync(id);
        }

        public async Task<List<PartyMaster>> GetAllPartiesAsync(string? partyType)
        {
            var parties = new List<PartyMaster>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetAllParties", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PartyType", SqlDbType.NVarChar, 100).Value = (object?)partyType ?? DBNull.Value; 

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                parties.Add(new PartyMaster
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? "",
                    Type = reader["Type"].ToString() ?? "",
                });
            }
            
            return parties;
        }

        public async Task<PartyMaster> SavePartyAsync(PartyMaster party)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_insertParty", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PartyName", SqlDbType.NVarChar, 100).Value = party.Name;
            cmd.Parameters.Add("@PartyType", SqlDbType.NVarChar, 100).Value = party.Type;
            // Add output parameter for new Id
            var idParam = cmd.Parameters.Add("@Id", SqlDbType.Int);
            idParam.Direction = ParameterDirection.Output;
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            // Set the generated Id back on the entity
            party.Id = (int)idParam.Value;

            return party;
        }

        public async Task<bool> UpdatePartyAsync(PartyMaster party)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_updateParty", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Id", SqlDbType.Int, 6).Value = party.Id;
            cmd.Parameters.Add("@PartyName", SqlDbType.NVarChar, 100).Value = party.Name;
            cmd.Parameters.Add("@PartyType", SqlDbType.NVarChar, 100).Value = party.Type;

            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeletePartyAsync(int Id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_deleteParty", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", Id);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
