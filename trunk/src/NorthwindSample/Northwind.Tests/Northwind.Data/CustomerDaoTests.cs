﻿using NUnit.Framework;
using SharpArch.Core.PersistenceSupport;
using Northwind.Core;
using Northwind.Data;
using System.Diagnostics;
using SharpArch.Core;
using NUnit.Framework.SyntaxHelpers;
using Northwind.Core.DataInterfaces;
using SharpArch.Data.NHibernate;
using System.Collections.Generic;
using System;

namespace Tests.Northwind.Data
{
    [TestFixture]
    [Category("DB Tests")]
    public class CustomerDaoTests : DaoTests
    {
        [Test]
        public void CanGetAllCustomers() {
            IList<Customer> customers = customerDao.LoadAll();

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers, Is.Not.Empty);
        }

        [Test]
        public void CanGetCustomerById() {
            Customer customer = GetCustomerById();

            Assert.That(customer.CompanyName, Is.EqualTo("Rancho grande"));
        }

        [Test]
        public void CanGetCustomerByProperties() {
            IDictionary<string, object> propertyValues = new Dictionary<string, object>();
            propertyValues.Add("CompanyName", "Rancho grande");
            Customer customer = customerDao.GetUniqueByProperties(propertyValues);

            Assert.That(customer, Is.Not.Null);
            Assert.That(customer.CompanyName, Is.EqualTo("Rancho grande"));

            propertyValues.Add("ContactName", "Won't Match");
            customer = customerDao.GetUniqueByProperties(propertyValues);

            Assert.That(customer, Is.Null);
        }

        /// <summary>
        /// This test demonstrates that the orders collection is lazily loaded.  Since lazilly loaded
        /// collections depend on a persisted session, this method is wrapped in a rolled-back transaction 
        /// managed via the superclass.
        /// </summary>
        [Test]
        public void CanLoadCustomerOrders() {
            Customer customer = GetCustomerById();

            Assert.That(customer.Orders.Count, Is.EqualTo(5));
        }

        [Test]
        public void CanFindCustomerOrdersViaCustomFilter() {
            Customer customer = GetCustomerById();

            Assert.That(customer.Orders.FindOrdersPlacedOn(new DateTime(1998, 1, 13)).Count, Is.EqualTo(1));
            Assert.That(customer.Orders.FindOrdersPlacedOn(new DateTime(1992, 10, 13)), Is.Empty);
        }

        /// <summary>
        /// It would be more effecient to use an example object or a more direct means to find the customers
        /// within a particular country, but this is a good demonstration of how the DAO extensions work.
        /// </summary>
        [Test]
        public void CanGetCustomersByCountry() {
            List<Customer> customers = customerDao.FindByCountry("Brazil");

            Assert.That(customers, Is.Not.Null);
            Assert.That(customers.Count, Is.EqualTo(9));
        }

        [Test]
        [ExpectedException(typeof(PreconditionException))]
        public void CanNotSaveOrUpdatePersistentObjectWithAssignedId() {
            Customer customer = GetCustomerById();
            customerDao.SaveOrUpdate(customer);
        }

        private Customer GetCustomerById() {
            Customer customer = customerDao.Load("RANCH", SharpArch.Core.Enums.LockMode.Read);
            Assert.That(customer, Is.Not.Null);
            return customer;
        }

        private ICustomerDao customerDao = new CustomerDao();
    }
}