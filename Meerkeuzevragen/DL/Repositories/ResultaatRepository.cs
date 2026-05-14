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
    public class ResultaatRepository : IResultaatRepository
    {
        private readonly string connectionString;

        public ResultaatRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void VoegTestResultaatToe(TestResultaat testResultaat)
        {
            string sqlResultaat = @"INSERT INTO TestResultaat
                                    (test_id, gebruiker_id, score, totaal_aantal_vragen, uitgevoerd_op)
                                    OUTPUT INSERTED.test_resultaat_id
                                    VALUES (@test_id, @gebruiker_id, @score, @totaal_aantal_vragen, @uitgevoerd_op)";

            string sqlGebruikerAntwoord = @"INSERT INTO GebruikerAntwoord
                                            (test_resultaat_id, test_vraag_id, gekozen_letter, is_correct)
                                            VALUES (@test_resultaat_id, @test_vraag_id, @gekozen_letter, @is_correct)";

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                using SqlCommand cmdResultaat = new SqlCommand(sqlResultaat, conn, transaction);

                cmdResultaat.Parameters.Add("@test_id", SqlDbType.Int).Value = testResultaat.Test.TestId;
                cmdResultaat.Parameters.Add("@gebruiker_id", SqlDbType.Int).Value = testResultaat.Gebruiker.GebruikerId;
                cmdResultaat.Parameters.Add("@score", SqlDbType.Int).Value = testResultaat.Score;
                cmdResultaat.Parameters.Add("@totaal_aantal_vragen", SqlDbType.Int).Value = testResultaat.TotaalAantalVragen;
                cmdResultaat.Parameters.Add("@uitgevoerd_op", SqlDbType.DateTime2).Value = testResultaat.UitgevoerdOp;

                int testResultaatId = (int)cmdResultaat.ExecuteScalar();

                foreach (GebruikerAntwoord gebruikerAntwoord in testResultaat.GebruikerAntwoorden)
                {
                    using SqlCommand cmdGebruikerAntwoord = new SqlCommand(sqlGebruikerAntwoord, conn, transaction);

                    cmdGebruikerAntwoord.Parameters.Add("@test_resultaat_id", SqlDbType.Int).Value = testResultaatId;
                    cmdGebruikerAntwoord.Parameters.Add("@test_vraag_id", SqlDbType.Int).Value = gebruikerAntwoord.TestVraag.TestVraagId;
                    cmdGebruikerAntwoord.Parameters.Add("@gekozen_letter", SqlDbType.Char).Value = gebruikerAntwoord.GekozenLetter;
                    cmdGebruikerAntwoord.Parameters.Add("@is_correct", SqlDbType.Bit).Value = gebruikerAntwoord.IsCorrect;

                    cmdGebruikerAntwoord.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Fout bij toevoegen testresultaat.", ex);
            }
        }

        public TestResultaat? GeefTestResultaatById(int testResultaatId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT tr.test_resultaat_id, tr.score, tr.totaal_aantal_vragen, tr.uitgevoerd_op,
                                  t.test_id, t.naam AS test_naam, t.aangemaakt_op, t.aantal_antwoorden_per_vraag,
                                  o.onderwerp_id, o.naam AS onderwerp_naam,
                                  g.gebruiker_id, g.naam AS gebruiker_naam
                           FROM TestResultaat tr
                           JOIN Test t ON tr.test_id = t.test_id
                           JOIN Onderwerp o ON t.onderwerp_id = o.onderwerp_id
                           JOIN Gebruiker g ON tr.gebruiker_id = g.gebruiker_id
                           WHERE tr.test_resultaat_id = @test_resultaat_id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@test_resultaat_id", SqlDbType.Int).Value = testResultaatId;

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            TestResultaat resultaat = MapTestResultaat(reader);

            reader.Close();

            LaadGebruikerAntwoorden(conn, resultaat);

            return resultaat;
        }

        public IReadOnlyList<TestResultaat> GeefResultatenByTest(int testId)
        {
            List<TestResultaat> resultaten = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT tr.test_resultaat_id, tr.score, tr.totaal_aantal_vragen, tr.uitgevoerd_op,
                                  t.test_id, t.naam AS test_naam, t.aangemaakt_op, t.aantal_antwoorden_per_vraag,
                                  o.onderwerp_id, o.naam AS onderwerp_naam,
                                  g.gebruiker_id, g.naam AS gebruiker_naam
                           FROM TestResultaat tr
                           JOIN Test t ON tr.test_id = t.test_id
                           JOIN Onderwerp o ON t.onderwerp_id = o.onderwerp_id
                           JOIN Gebruiker g ON tr.gebruiker_id = g.gebruiker_id
                           WHERE tr.test_id = @test_id
                           ORDER BY tr.uitgevoerd_op DESC";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@test_id", SqlDbType.Int).Value = testId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                resultaten.Add(MapTestResultaat(reader));
            }

            return resultaten;
        }

        public IReadOnlyList<TestResultaat> GeefResultatenByGebruiker(int gebruikerId)
        {
            List<TestResultaat> resultaten = new();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"SELECT tr.test_resultaat_id, tr.score, tr.totaal_aantal_vragen, tr.uitgevoerd_op,
                                  t.test_id, t.naam AS test_naam, t.aangemaakt_op, t.aantal_antwoorden_per_vraag,
                                  o.onderwerp_id, o.naam AS onderwerp_naam,
                                  g.gebruiker_id, g.naam AS gebruiker_naam
                           FROM TestResultaat tr
                           JOIN Test t ON tr.test_id = t.test_id
                           JOIN Onderwerp o ON t.onderwerp_id = o.onderwerp_id
                           JOIN Gebruiker g ON tr.gebruiker_id = g.gebruiker_id
                           WHERE tr.gebruiker_id = @gebruiker_id
                           ORDER BY tr.uitgevoerd_op DESC";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@gebruiker_id", SqlDbType.Int).Value = gebruikerId;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                resultaten.Add(MapTestResultaat(reader));
            }

            return resultaten;
        }

        private TestResultaat MapTestResultaat(SqlDataReader reader)
        {
            Onderwerp onderwerp = new Onderwerp(
                (int)reader["onderwerp_id"],
                reader["onderwerp_naam"].ToString()!
            );

            Test test = new Test(
                (int)reader["test_id"],
                reader["test_naam"].ToString()!,
                (DateTime)reader["aangemaakt_op"],
                onderwerp,
                (int)reader["aantal_antwoorden_per_vraag"]
            );

            Gebruiker gebruiker = new Gebruiker(
                (int)reader["gebruiker_id"],
                reader["gebruiker_naam"].ToString()!
            );

            return new TestResultaat(
                (int)reader["test_resultaat_id"],
                test,
                gebruiker,
                (int)reader["score"],
                (int)reader["totaal_aantal_vragen"],
                (DateTime)reader["uitgevoerd_op"]
            );
        }

        private void LaadGebruikerAntwoorden(SqlConnection conn, TestResultaat resultaat)
        {
            string sql = @"SELECT ga.gebruiker_antwoord_id, ga.gekozen_letter, ga.is_correct,
                                  tv.test_vraag_id, tv.volgorde AS testvraag_volgorde,
                                  v.vraag_id, v.tekst, v.is_beschikbaar,
                                  o.onderwerp_id, o.naam AS onderwerp_naam
                           FROM GebruikerAntwoord ga
                           JOIN TestVraag tv ON ga.test_vraag_id = tv.test_vraag_id
                           JOIN Vraag v ON tv.vraag_id = v.vraag_id
                           JOIN Onderwerp o ON v.onderwerp_id = o.onderwerp_id
                           WHERE ga.test_resultaat_id = @test_resultaat_id
                           ORDER BY tv.volgorde";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@test_resultaat_id", SqlDbType.Int).Value = resultaat.TestResultaatId;

            using SqlDataReader reader = cmd.ExecuteReader();

            List<GebruikerAntwoord> gebruikerAntwoorden = new();

            while (reader.Read())
            {
                Onderwerp onderwerp = new Onderwerp(
                    (int)reader["onderwerp_id"],
                    reader["onderwerp_naam"].ToString()!
                );

                Vraag vraag = new Vraag(
                    (int)reader["vraag_id"],
                    reader["tekst"].ToString()!,
                    (bool)reader["is_beschikbaar"],
                    onderwerp
                );

                TestVraag testVraag = new TestVraag(
                    (int)reader["test_vraag_id"],
                    resultaat.Test,
                    vraag,
                    (int)reader["testvraag_volgorde"]
                );

                GebruikerAntwoord gebruikerAntwoord = new GebruikerAntwoord(
                    (int)reader["gebruiker_antwoord_id"],
                    resultaat,
                    testVraag,
                    Convert.ToChar(reader["gekozen_letter"]),
                    (bool)reader["is_correct"]
                );

                gebruikerAntwoorden.Add(gebruikerAntwoord);
            }

            reader.Close();

            foreach (GebruikerAntwoord gebruikerAntwoord in gebruikerAntwoorden)
            {
                resultaat.LaadGebruikerAntwoordToe(gebruikerAntwoord);
            }
        }
    }
}
