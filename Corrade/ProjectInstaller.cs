﻿using System.ComponentModel;
using System.Configuration.Install;


namespace Corrade
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            // Set the service name.
            string serviceName = string.IsNullOrEmpty(Corrade.CorradeServiceName) ? CorradeInstaller.ServiceName : Corrade.CorradeServiceName;
            CorradeInstaller.ServiceName = serviceName;
            CorradeInstaller.DisplayName = serviceName;
        }
    }
}
