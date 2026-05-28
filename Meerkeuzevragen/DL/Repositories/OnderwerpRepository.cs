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
    public class OnderwerpRepository : IOnderwerpRepository
    {
        private readonly string connectionString;

        public OnderwerpRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void VoegOnderwerpToe(Onderwerp onderwerp)
        {
            string sql = @"INSERT INTO Onderwerp (naam)
                           VALUES (@naam)";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@naam", SqlDbType.NVarChar).Value = onderwerp.Naam;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij toevoegen onderwerp.", ex);
            }
        }

        public Onderwerp GeefOnderwerpById(int onderwerpId)
        {
            string sql = @"SELECT onderwerp_id, naam
                           FROM Onderwerp
                           WHERE onderwerp_id = @onderwerp_id";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = onderwerpId;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapOnderwerp(reader);
        }

        public Onderwerp? GeefOnderwerpByNaam(string naam)
        {
            string sql = @"SELECT onderwerp_id, naam
                           FROM Onderwerp
                           WHERE naam = @naam";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@naam", SqlDbType.NVarChar).Value = naam;

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapOnderwerp(reader);
        }

        public List<Onderwerp> GeefOnderwerpen()   
        {
            List<Onderwerp> onderwerpen = new();

            string sql = @"SELECT onderwerp_id, naam
                           FROM Onderwerp
                           ORDER BY naam";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            conn.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                onderwerpen.Add(MapOnderwerp(reader));
            }

            return onderwerpen;
        }

        public bool BestaatOnderwerpMetNaam(string naam)
        {
            string sql = @"SELECT COUNT(*)
                           FROM Onderwerp
                           WHERE naam = @naam";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@naam", SqlDbType.NVarChar).Value = naam;

            conn.Open();

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        private Onderwerp MapOnderwerp(SqlDataReader reader)
        {
            return new Onderwerp(
                (int)reader["onderwerp_id"],
                reader["naam"].ToString()!
            );
        }
    }
}
