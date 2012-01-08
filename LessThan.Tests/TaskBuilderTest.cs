using LessThan.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using LessThan.Models;

namespace LessThan.Tests
{
    [TestClass()]
    public class TaskBuilderTest
    {
        [TestMethod()]
        public void CreateTaskTest1()
        {
            string input = "#LT ^2011-01-01 @Luuk ! =3h Aanmoedigen om te testen";
            var actual = TaskBuilder.CreateTask(input);
            Assert.AreEqual("LT", actual.Project);
            Assert.AreEqual(new DateTime(2011, 01, 01), actual.DueDate);
            Assert.AreEqual("Luuk", actual.AssignedTo);
            Assert.AreEqual(new TimeSpan(0, 3, 0), actual.EstimatedTime);
            Assert.AreEqual("Aanmoedigen om te testen", actual.TaskDescription);
        }

        [TestMethod()]
        public void CreateTaskTest2()
        {
            string input = "#LT1 ^2011-01-24 @Luuk =5m Nou dit werkt goed";
            var actual = TaskBuilder.CreateTask(input);
            Assert.AreEqual("LT1", actual.Project);
            Assert.AreEqual(new DateTime(2011, 01, 24), actual.DueDate);
            Assert.AreEqual("Luuk", actual.AssignedTo);
            Assert.AreEqual(new TimeSpan(0, 0, 5), actual.EstimatedTime);
            Assert.AreEqual("Nou dit werkt goed", actual.TaskDescription);
        }

        [TestMethod()]
        public void CreateTaskTest3()
        {
            string input = "#LT ^2011-11-01 @Luuk2 =3.5h Nou dit werkt goed";
            var actual = TaskBuilder.CreateTask(input);
            Assert.AreEqual("LT", actual.Project);
            Assert.AreEqual(new DateTime(2011, 11, 01), actual.DueDate);
            Assert.AreEqual("Luuk2", actual.AssignedTo);
            Assert.AreEqual(new TimeSpan(0, 3, 30), actual.EstimatedTime);
            Assert.AreEqual("Nou dit werkt goed", actual.TaskDescription);
        }
    }
}

