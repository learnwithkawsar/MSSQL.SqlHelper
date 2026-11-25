using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Xunit;

namespace MSSQL.SqlHelper.Tests
{
    // CommandTimeout is static, so tests that change it must not run in parallel with each other.
    [Collection("SqlHelper static state")]
    public class SqlHelperTests
    {
        [Fact]
        public void CommandTimeout_DefaultsToZero()
        {
            Assert.Equal(0, SqlHelper.CommandTimeout);
        }

        [Fact]
        public void UpdateCommandTimeout_AppliesConfiguredValue()
        {
            var original = SqlHelper.CommandTimeout;
            try
            {
                SqlHelper.CommandTimeout = 45;
                using (var command = new SqlCommand())
                {
                    SqlHelper.UpdateCommandTimeout(command);
                    Assert.Equal(45, command.CommandTimeout);
                }
            }
            finally
            {
                SqlHelper.CommandTimeout = original;
            }
        }

        [Fact]
        public void UpdateCommandTimeout_IgnoresNullCommand()
        {
            SqlHelper.UpdateCommandTimeout(null);
        }

        [Fact]
        public void ToQuery_FormatsProcedureCall()
        {
            Assert.Equal("dbo.GetUser  1 , abc ", SqlHelper.ToQuery("dbo.GetUser", 1, "abc"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ExecuteNonQuery_RejectsMissingConnectionString(string connectionString)
        {
            Assert.Throws<ArgumentNullException>(() =>
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "SELECT 1"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ExecuteDataTable_RejectsMissingConnectionString(string connectionString)
        {
            Assert.Throws<ArgumentNullException>(() =>
                SqlHelper.ExecuteDataTable(connectionString, CommandType.Text, "SELECT 1"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ExecuteScalar_RejectsMissingConnectionString(string connectionString)
        {
            Assert.Throws<ArgumentNullException>(() =>
                SqlHelper.ExecuteScalar(connectionString, CommandType.Text, "SELECT 1"));
        }

        [Fact]
        public void ExecuteNonQuery_RejectsNullConnection()
        {
            Assert.Throws<ArgumentNullException>(() =>
                SqlHelper.ExecuteNonQuery((SqlConnection)null, CommandType.Text, "SELECT 1"));
        }
    }
}
