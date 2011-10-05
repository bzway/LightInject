﻿using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using LightInject.SampleLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependencyInjector.Tests
{
    [TestClass]
    public class AssemblyScannerTests
    {
        private Mock<IServiceContainer> GetContainerMock()
        {
            var containerMock = new Mock<IServiceContainer>();
            var assemblyScanner = new AssemblyScanner(containerMock.Object);
            assemblyScanner.Scan(typeof(IFoo).Assembly, (t) => true);
            return containerMock;
        }
        
        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultService()
        {            
            GetContainerMock().Verify(sc => sc.Register(typeof(IFoo), typeof(Foo), string.Empty), Times.Once());
        }
        
        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedService()
        {            
            GetContainerMock().Verify(sc => sc.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo"), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresDefaultOpenGenericService()
        {
            GetContainerMock().Verify(sc => sc.Register(typeof(IFoo<>), typeof(Foo<>), string.Empty), Times.Once());
        }

        [TestMethod]
        public void Scan_SampleAssembly_ConfiguresNamedOpenGenericType()
        {
            GetContainerMock().Verify(sc => sc.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo"), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetInstance_NoServices_CallsAssemblyScannerOnFirstRequest()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.GetInstance<IFoo>();
            serviceContainer.GetInstance<IFoo>();
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly,(t) => true),Times.Once());
        }

        [TestMethod]
        public void Register_AssemblyFile_CallsAssemblyScanner()
        {
            var scannerMock = new Mock<IAssemblyScanner>();
            var serviceContainer = new ServiceContainer();
            serviceContainer.AssemblyScanner = scannerMock.Object;
            serviceContainer.Register("*SampleLibrary.dll",t => true);
            scannerMock.Verify(a => a.Scan(typeof(IFoo).Assembly, It.IsAny<Func<Type,bool>>()), Times.Once());
        }
    }
}
