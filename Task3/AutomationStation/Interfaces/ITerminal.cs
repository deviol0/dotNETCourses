using System;
using AutomationStation.Models;
using AutomationStation.Responds;

namespace AutomationStation.Interfaces
{
    public interface ITerminal
    {
        PhoneNumber Number { get; }
        Port Port { get; }
        void Call(PhoneNumber target);
        void Answer();
        void Decline();
    }
}