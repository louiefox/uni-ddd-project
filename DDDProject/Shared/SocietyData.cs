using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DDDProject.Data
{
    public struct SocietyData
    {
        public string SocietyID;
        public string Name;
        public string Icon;
        public bool IsMemberOf;
        public bool IsAdministrator;
    }

    public struct EventData
    {
        public string Name;
        public string SocietyID;
        public string SocietyName;
        public string DateLocation;
    }

    public struct AnnouncementData
    {
        public string Date;
        public string Content;
    }

    public struct MemberData
    {
        public string Username;
        public string Fullname;
        public bool IsAdministrator;
    }
}