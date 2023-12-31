﻿// Copyright (c) 2016, 2019, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MySql.Data.Entity.CodeFirst.Tests
{
  public class TransactionTests : IClassFixture<CodeFirstFixture>
  {
    private CodeFirstFixture _fixture;

    public TransactionTests(CodeFirstFixture fixture)
    {
      _fixture = fixture;
      _fixture.Setup(this.GetType());
    }

    [Fact]
    public void DisposeNestedTransactions()
    {
      using (SakilaDb context = new SakilaDb())
      {
        using (var trans = context.Database.BeginTransaction())
        {
          Assert.Throws<MySqlException>(() => context.Database.ExecuteSqlCommand("update abc"));
        }
      }

      // new second transaction
      using (SakilaDb context = new SakilaDb())
      {
        using (var trans = context.Database.BeginTransaction())
        {
          Assert.Throws<MySqlException>(() => context.Database.ExecuteSqlCommand("update abc"));
        }
      }
    }

    [Fact]
    public void NestedTransactionsUniqueKey()
    {
      using (SakilaDb context = new SakilaDb())
      {
        var store = new store
        {
          manager_staff_id = 1
        };
        context.stores.Add(store);
        for (int i = 0; i < 10; i++)
        {
          Assert.Throws<DbUpdateException>(() => context.SaveChanges());
        }
      }
    }
  }
}
