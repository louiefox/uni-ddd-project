using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DDDProject.Data
{
    public struct SocietyData
    {
        public string Name;
        public string Icon;
    }

    public struct EventData
    {
        public string Name;
        public string SocietyID;
        public string SocietyName;
        public string DateLocation;
    }
}