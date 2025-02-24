using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMusicStore
{
    public static class Connection
    {
        public const string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=OnlineMusicStore;Integrated Security=True;Encrypt=False;Trust Server Certificate=True";
    }
}
