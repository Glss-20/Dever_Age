using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TesteHTML
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        private static string conexaoString = "server=localhost; database=banco_projeto_age; user=root; password=Glss;";
        private static string caminhoHTML = @"D:\Documents\ESTUDOS_LACERDA\Projetos\Dever de casa - Agezandro\TesteHTML\Pagina2_Aluno.html";

        public Aluno()
        {

        }

        public Aluno(int id, string nome)
        {
            this.Id = id;
            this.Nome = nome;
        }


        public static void Cadastro(string dados)
        {
            var parametros = System.Web.HttpUtility.ParseQueryString(dados);
            int id = int.Parse(parametros["id"]);
            string nome = parametros["nome"];

            Aluno aluno = new Aluno(id, nome);
            aluno.ExibirCadastro();
            aluno.SalvarBanco();
        }

        public static string PaginaAlunos()
        {
            if (!File.Exists(caminhoHTML))
            {
                return "<h2>Erro: Página não encontrada!</h2>";
            }

            string html = File.ReadAllText(caminhoHTML, Encoding.UTF8);
            StringBuilder listaHtml = new StringBuilder();

            List<Aluno> alunos = Aluno.ListarAlunos();

            if (alunos.Count == 0)
            {
                listaHtml.AppendLine("<li>Nenhum aluno cadastrado.</li>");
            }
            else
            {
                foreach (Aluno aluno in alunos)
                {
                    listaHtml.AppendLine($"<li>ID: {aluno.Id} - Nome: {aluno.Nome}</li>");
                }
            }


            return html.Replace("<!--Alunos-->", $"<ul>{listaHtml}</ul>");
        }

        public static List<Aluno> ListarAlunos()
        {
            List<Aluno> alunos = new List<Aluno>();

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    conexao.Open();
                    Console.WriteLine("✅ Conexão com o banco aberta!");

                    string query = "SELECT id, nome FROM alunos";

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        while (leitor.Read())
                        {
                            var aluno = new Aluno(leitor.GetInt32("id"), leitor.GetString("nome"));
                            alunos.Add(aluno);
                            Console.WriteLine($"📌 Lido do banco: {aluno.Id} - {aluno.Nome}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erro ao buscar alunos: " + ex.Message);
            }

            return alunos;
        }


        public void ExibirCadastro()
        {
            Console.WriteLine($"ID = {Id} - Nome = {Nome}");
        }

        public void SalvarBanco()
        {
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    conexao.Open();
                    string query = "INSERT INTO alunos (id, nome) VALUES (@id, @nome)";

                    using (MySqlCommand comando = new MySqlCommand(query, conexao))
                    {
                        comando.Parameters.AddWithValue("@id", Id);
                        comando.Parameters.AddWithValue("@nome", Nome);
                        comando.ExecuteNonQuery();
                    }

                }
                Console.WriteLine("Aluno Cadastrado com sucesso!");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Erro ao salvar no banco: {ex.Message}");
            }
        }
    }
}
