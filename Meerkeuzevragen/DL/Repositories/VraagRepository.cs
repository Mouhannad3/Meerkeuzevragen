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
    public class VraagRepository : IVraagRepository
    {
        private readonly string connectionString;

        public VraagRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void VoegVraagToe(Vraag vraag)
        {
            string sqlVraag = @"INSERT INTO Vraag (onderwerp_id, tekst, is_beschikbaar)
                                OUTPUT INSERTED.vraag_id
                                VALUES (@onderwerp_id, @tekst, @is_beschikbaar)";

            string sqlAntwoord = @"INSERT INTO Antwoord (vraag_id, tekst, is_correct)
                                   VALUES (@vraag_id, @tekst, @is_correct)";

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                using SqlCommand cmdVraag = new SqlCommand(sqlVraag, conn, transaction);

                cmdVraag.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = vraag.Onderwerp.OnderwerpId;
                cmdVraag.Parameters.Add("@tekst", SqlDbType.NVarChar).Value = vraag.Tekst;
                cmdVraag.Parameters.Add("@is_beschikbaar", SqlDbType.Bit).Value = vraag.IsBeschikbaar;

                int vraagId = (int)cmdVraag.ExecuteScalar();

                foreach (Antwoord antwoord in vraag.Antwoorden)
                {
                    using SqlCommand cmdAntwoord = new SqlCommand(sqlAntwoord, conn, transaction);

                    cmdAntwoord.Parameters.Add("@vraag_id", SqlDbType.Int).Value = vraagId;
                    cmdAntwoord.Parameters.Add("@tekst", SqlDbType.NVarChar).Value = antwoord.Tekst;
                    cmdAntwoord.Parameters.Add("@is_correct", SqlDbType.Bit).Value = antwoord.IsCorrect;

                    cmdAntwoord.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fout bij toevoegen vraag.", ex);
            }
        }

        public void UpdateVraag(Vraag vraag)
        {
            string sqlVraag = @"UPDATE Vraag
                                SET tekst = @tekst,
                                    is_beschikbaar = @is_beschikbaar,
                                    onderwerp_id = @onderwerp_id
                                WHERE vraag_id = @vraag_id";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sqlVraag, conn);

            cmd.Parameters.Add("@vraag_id", SqlDbType.Int).Value = vraag.VraagId;
            cmd.Parameters.Add("@tekst", SqlDbType.NVarChar).Value = vraag.Tekst;
            cmd.Parameters.Add("@is_beschikbaar", SqlDbType.Bit).Value = vraag.IsBeschikbaar;
            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = vraag.Onderwerp.OnderwerpId;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij updaten vraag.", ex);
            }
        }

        public Vraag? GeefVraagById(int vraagId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT v.vraag_id, v.tekst, v.is_beschikbaar,
                                  o.onderwerp_id, o.naam
                           FROM Vraag v
                           JOIN Onderwerp o ON v.onderwerp_id = o.onderwerp_id
                           WHERE v.vraag_id = @vraag_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@vraag_id", SqlDbType.Int).Value = vraagId;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            Vraag vraag = MapVraag(reader);

            reader.Close();

            LaadAntwoorden(conn, vraag);

            return vraag;
        }

        public IReadOnlyList<Vraag> GeefVragenByOnderwerp(int onderwerpId)
        {
            List<Vraag> vragen = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT v.vraag_id, v.tekst, v.is_beschikbaar,
                                  o.onderwerp_id, o.naam
                           FROM Vraag v
                           JOIN Onderwerp o ON v.onderwerp_id = o.onderwerp_id
                           WHERE v.onderwerp_id = @onderwerp_id
                           ORDER BY v.vraag_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = onderwerpId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                vragen.Add(MapVraag(reader));
            }

            reader.Close();

            foreach (Vraag vraag in vragen)
            {
                LaadAntwoorden(conn, vraag);
            }

            return vragen;
        }

        public IReadOnlyList<Vraag> GeefBeschikbareVragenByOnderwerp(int onderwerpId)
        {
            List<Vraag> vragen = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT v.vraag_id, v.tekst, v.is_beschikbaar,
                                  o.onderwerp_id, o.naam
                           FROM Vraag v
                           JOIN Onderwerp o ON v.onderwerp_id = o.onderwerp_id
                           WHERE v.onderwerp_id = @onderwerp_id
                             AND v.is_beschikbaar = 1
                           ORDER BY v.vraag_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = onderwerpId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                vragen.Add(MapVraag(reader));
            }

            reader.Close();

            foreach (Vraag vraag in vragen)
            {
                LaadAntwoorden(conn, vraag);
            }

            return vragen;
        }

        public IReadOnlyList<Vraag> GeefBeschikbareVragenByOnderwerpEnAantalAntwoorden(int onderwerpId, int aantalAntwoorden)
        {
            List<Vraag> vragen = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT v.vraag_id, v.tekst, v.is_beschikbaar,
                                  o.onderwerp_id, o.naam
                           FROM Vraag v
                           JOIN Onderwerp o ON v.onderwerp_id = o.onderwerp_id
                           WHERE v.onderwerp_id = @onderwerp_id
                             AND v.is_beschikbaar = 1
                             AND (
                                 SELECT COUNT(*)
                                 FROM Antwoord a
                                 WHERE a.vraag_id = v.vraag_id
                             ) = @aantal_antwoorden
                           ORDER BY v.vraag_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = onderwerpId;
            cmd.Parameters.Add("@aantal_antwoorden", SqlDbType.Int).Value = aantalAntwoorden;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                vragen.Add(MapVraag(reader));
            }

            reader.Close();

            foreach (Vraag vraag in vragen)
            {
                LaadAntwoorden(conn, vraag);
            }

            return vragen;
        }

        public bool BestaatVraagMetTekst(string tekst, int onderwerpId, int? vraagId)
        {
            string sql = @"SELECT COUNT(*)
                           FROM Vraag
                           WHERE tekst = @tekst
                             AND onderwerp_id = @onderwerp_id";

            if (vraagId.HasValue)
            {
                sql += " AND vraag_id <> @vraag_id";
            }

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@tekst", SqlDbType.NVarChar).Value = tekst;
            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = onderwerpId;

            if (vraagId.HasValue)
            {
                cmd.Parameters.Add("@vraag_id", SqlDbType.Int).Value = vraagId.Value;
            }

            conn.Open();

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        private Vraag MapVraag(SqlDataReader reader)
        {
            Onderwerp onderwerp = new Onderwerp(
                (int)reader["onderwerp_id"],
                reader["naam"].ToString()!
            );

            return new Vraag(
                (int)reader["vraag_id"],
                reader["tekst"].ToString()!,
                (bool)reader["is_beschikbaar"],
                onderwerp
            );
        }

        private void LaadAntwoorden(SqlConnection conn, Vraag vraag)
        {
            string sql = @"SELECT antwoord_id, tekst, is_correct
                           FROM Antwoord
                           WHERE vraag_id = @vraag_id
                           ORDER BY antwoord_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@vraag_id", SqlDbType.Int).Value = vraag.VraagId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Antwoord antwoord = new Antwoord(
                    (int)reader["antwoord_id"],
                    reader["tekst"].ToString()!,
                    (bool)reader["is_correct"]
                );

                vraag.VoegAntwoordToe(antwoord);
            }
        }
    }
}
