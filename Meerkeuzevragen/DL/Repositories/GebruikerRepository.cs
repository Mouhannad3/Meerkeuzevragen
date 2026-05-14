using BL.Domein;
using BL.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Repositories
{
    public class GebruikerRepository : IGebruikerRepository
    {
        private readonly string connectionString;

        public GebruikerRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void VoegGebruikerToe(Gebruiker gebruiker)
        {
            string sql = @"INSERT INTO Gebruiker (naam)
                           VALUES (@naam)";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@naam", SqlDbType.NVarChar).Value = gebruiker.Naam;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij toevoegen gebruiker.", ex);
            }
        }

        public Gebruiker? GeefGebruikerById(int gebruikerId)
        {
            string sql = @"SELECT gebruiker_id, naam
                           FROM Gebruiker
                           WHERE gebruiker_id = @gebruiker_id";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@gebruiker_id", SqlDbType.Int).Value = gebruikerId;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapGebruiker(reader);
        }

        public bool BestaatGebruiker(int gebruikerId)
        {
            string sql = @"SELECT COUNT(*)
                           FROM Gebruiker
                           WHERE gebruiker_id = @gebruiker_id";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@gebruiker_id", SqlDbType.Int).Value = gebruikerId;

            conn.Open();

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        private Gebruiker MapGebruiker(SqlDataReader reader)
        {
            return new Gebruiker(
                (int)reader["gebruiker_id"],
                reader["naam"].ToString()!
            );
        }
    }
}
