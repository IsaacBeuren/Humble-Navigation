using Br.Scania.ExternalAGV.Model.DataBase;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Br.Scania.ExternalAGV.Dapper
{
    public class AgvDAO
    {
        private IConfiguration _configuracoes;
        public AgvDAO(IConfiguration config)
        {
            _configuracoes = config;
        }

        public ConfigModel GetAll(int ID)
        {
            using (SqlConnection conexao = new SqlConnection(
                _configuracoes.GetConnectionString("connectionString")))
            {
                return conexao.QueryFirstOrDefault<ConfigModel>(
                    "SELECT " +
                        "ID, " +
                        "Prefix, " +
                        "IDAGV, " +
                    "FROM dbo.Config " +
                    "WHERE ID = ID ",
                    new { IDx = ID });
            }
        }
    }
}