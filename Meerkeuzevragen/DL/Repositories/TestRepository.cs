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
    public class TestRepository : ITestRepository
    {
        private readonly string connectionString;

        public TestRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void VoegTestToe(Test test)
        {
            string sqlTest = @"INSERT INTO Test (onderwerp_id, naam, aangemaakt_op, aantal_antwoorden_per_vraag)
                               OUTPUT INSERTED.test_id
                               VALUES (@onderwerp_id, @naam, @aangemaakt_op, @aantal_antwoorden_per_vraag)";

            string sqlTestVraag = @"INSERT INTO TestVraag (test_id, vraag_id, volgorde)
                                    OUTPUT INSERTED.test_vraag_id
                                    VALUES (@test_id, @vraag_id, @volgorde)";

            string sqlTestVraagAntwoord = @"INSERT INTO TestVraagAntwoord
                                            (test_vraag_id, antwoord_id, letter, volgorde)
                                            VALUES (@test_vraag_id, @antwoord_id, @letter, @volgorde)";

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                using SqlCommand cmdTest = new SqlCommand(sqlTest, conn, transaction);

                cmdTest.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = test.Onderwerp.OnderwerpId;
                cmdTest.Parameters.Add("@naam", SqlDbType.NVarChar).Value = test.Naam;
                cmdTest.Parameters.Add("@aangemaakt_op", SqlDbType.DateTime2).Value = test.AangemaaktOp;
                cmdTest.Parameters.Add("@aantal_antwoorden_per_vraag", SqlDbType.Int).Value = test.AantalAntwoordenPerVraag;

                int testId = (int)cmdTest.ExecuteScalar();

                foreach (TestVraag testVraag in test.TestVragen)
                {
                    using SqlCommand cmdTestVraag = new SqlCommand(sqlTestVraag, conn, transaction);

                    cmdTestVraag.Parameters.Add("@test_id", SqlDbType.Int).Value = testId;
                    cmdTestVraag.Parameters.Add("@vraag_id", SqlDbType.Int).Value = testVraag.Vraag.VraagId;
                    cmdTestVraag.Parameters.Add("@volgorde", SqlDbType.Int).Value = testVraag.Volgorde;

                    int testVraagId = (int)cmdTestVraag.ExecuteScalar();

                    foreach (TestVraagAntwoord testVraagAntwoord in testVraag.TestVraagAntwoorden)
                    {
                        using SqlCommand cmdTestVraagAntwoord = new SqlCommand(sqlTestVraagAntwoord, conn, transaction);

                        cmdTestVraagAntwoord.Parameters.Add("@test_vraag_id", SqlDbType.Int).Value = testVraagId;
                        cmdTestVraagAntwoord.Parameters.Add("@antwoord_id", SqlDbType.Int).Value = testVraagAntwoord.Antwoord.AntwoordId;
                        cmdTestVraagAntwoord.Parameters.Add("@letter", SqlDbType.Char).Value = testVraagAntwoord.Letter;
                        cmdTestVraagAntwoord.Parameters.Add("@volgorde", SqlDbType.Int).Value = testVraagAntwoord.Volgorde;

                        cmdTestVraagAntwoord.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fout bij toevoegen test.", ex);
            }
        }

        public Test? GeefTestById(int testId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT t.test_id, t.naam, t.aangemaakt_op, t.aantal_antwoorden_per_vraag,
                                  o.onderwerp_id, o.naam AS onderwerp_naam
                           FROM Test t
                           JOIN Onderwerp o ON t.onderwerp_id = o.onderwerp_id
                           WHERE t.test_id = @test_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@test_id", SqlDbType.Int).Value = testId;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            Test test = MapTest(reader);

            reader.Close();

            LaadTestVragen(conn, test);

            return test;
        }

        public IReadOnlyList<Test> GeefTesten()
        {
            List<Test> testen = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT t.test_id, t.naam, t.aangemaakt_op, t.aantal_antwoorden_per_vraag,
                                  o.onderwerp_id, o.naam AS onderwerp_naam
                           FROM Test t
                           JOIN Onderwerp o ON t.onderwerp_id = o.onderwerp_id
                           ORDER BY t.aangemaakt_op DESC";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                testen.Add(MapTest(reader));
            }

            return testen;
        }

        public IReadOnlyList<Test> GeefTestenByOnderwerp(int onderwerpId)
        {
            List<Test> testen = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT t.test_id, t.naam, t.aangemaakt_op, t.aantal_antwoorden_per_vraag,
                                  o.onderwerp_id, o.naam AS onderwerp_naam
                           FROM Test t
                           JOIN Onderwerp o ON t.onderwerp_id = o.onderwerp_id
                           WHERE t.onderwerp_id = @onderwerp_id
                           ORDER BY t.aangemaakt_op DESC";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@onderwerp_id", SqlDbType.Int).Value = onderwerpId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                testen.Add(MapTest(reader));
            }

            return testen;
        }

        public bool BestaatTestMetNaam(string naam)
        {
            string sql = @"SELECT COUNT(*)
                           FROM Test
                           WHERE naam = @naam";

            using SqlConnection conn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@naam", SqlDbType.NVarChar).Value = naam;

            conn.Open();

            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

        private Test MapTest(SqlDataReader reader)
        {
            Onderwerp onderwerp = new Onderwerp(
                (int)reader["onderwerp_id"],
                reader["onderwerp_naam"].ToString()!
            );

            return new Test(
                (int)reader["test_id"],
                reader["naam"].ToString()!,
                (DateTime)reader["aangemaakt_op"],
                onderwerp,
                (int)reader["aantal_antwoorden_per_vraag"]
            );
        }

        private void LaadTestVragen(SqlConnection conn, Test test)
        {
            string sql = @"SELECT tv.test_vraag_id, tv.volgorde,
                                  v.vraag_id, v.tekst, v.is_beschikbaar,
                                  o.onderwerp_id, o.naam
                           FROM TestVraag tv
                           JOIN Vraag v ON tv.vraag_id = v.vraag_id
                           JOIN Onderwerp o ON v.onderwerp_id = o.onderwerp_id
                           WHERE tv.test_id = @test_id
                           ORDER BY tv.volgorde";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@test_id", SqlDbType.Int).Value = test.TestId;

            using SqlDataReader reader = cmd.ExecuteReader();

            List<TestVraag> testVragen = new();

            while (reader.Read())
            {
                Onderwerp onderwerp = new Onderwerp(
                    (int)reader["onderwerp_id"],
                    reader["naam"].ToString()!
                );

                Vraag vraag = new Vraag(
                    (int)reader["vraag_id"],
                    reader["tekst"].ToString()!,
                    (bool)reader["is_beschikbaar"],
                    onderwerp
                );

                TestVraag testVraag = new TestVraag(
                    (int)reader["test_vraag_id"],
                    test,
                    vraag,
                    (int)reader["volgorde"]
                );

                testVragen.Add(testVraag);
            }

            reader.Close();

            foreach (TestVraag testVraag in testVragen)
            {
                LaadAntwoordenVanVraag(conn, testVraag.Vraag);
                LaadTestVraagAntwoorden(conn, testVraag);
                test.VoegTestVraagToe(testVraag);
            }
        }

        private void LaadTestVraagAntwoorden(SqlConnection conn, TestVraag testVraag)
        {
            string sql = @"SELECT tva.test_vraag_antwoord_id, tva.letter, tva.volgorde,
                                  a.antwoord_id, a.tekst, a.is_correct
                           FROM TestVraagAntwoord tva
                           JOIN Antwoord a ON tva.antwoord_id = a.antwoord_id
                           WHERE tva.test_vraag_id = @test_vraag_id
                           ORDER BY tva.volgorde";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@test_vraag_id", SqlDbType.Int).Value = testVraag.TestVraagId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Antwoord antwoord = new Antwoord(
                    (int)reader["antwoord_id"],
                    reader["tekst"].ToString()!,
                    (bool)reader["is_correct"]
                );

                TestVraagAntwoord testVraagAntwoord = new TestVraagAntwoord(
                    (int)reader["test_vraag_antwoord_id"],
                    testVraag,
                    antwoord,
                    Convert.ToChar(reader["letter"]),
                    (int)reader["volgorde"]
                );

                testVraag.VoegTestVraagAntwoordToe(testVraagAntwoord);
            }
        }
        private void LaadAntwoordenVanVraag(SqlConnection conn, Vraag vraag)
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
