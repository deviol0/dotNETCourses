using System;
using System.Collections.Generic;
using System.Linq;
using AutomationStation.Billing;
using AutomationStation.Exception;
using AutomationStation.Interfaces;
using AutomationStation.Requests;
using AutomationStation.Responds;

namespace AutomationStation.Models
{
    public class RequestHandler
    {
        private readonly List<Port> _portCollection;
        private readonly CallInfo _currentCall;

        public RequestHandler(PhoneNumber source,PhoneNumber target,List<Port> portCollection,double tariff)
        {
            _currentCall = new CallInfo(source,target,tariff);
            _portCollection = portCollection;
        }

        public CallInfo GetCallInfo()
        {
            return _currentCall;
        }
        private Port GetPortByNumber(PhoneNumber number)
        {
            if (number == null) throw new NullReferenceException();
            return _portCollection?.First(port => port.Terminal.Number == number);
        }
        
        public void CreateNewRequest(OutgoingRequest request)
        {
            var port = GetPortByNumber(request.Target);
            var message = "";
            switch (port.State)
            {
                case PortState.Busy:
                    message = "Subscriber is busy now";
                    break;
                case PortState.Disabled:
                    message = "Subscriber is not available now";
                    break;
                case PortState.Free:
                    port.State = PortState.Busy;
                    port.NewIncomingRequest(request.Source);
                    port.CallRespond += ((sender, respond) => GetRespond((Port) sender, respond));
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            port.NewStationRespond(new StationRespond()
                {Request = request, DeclineMessage = message, State = RespondState.Decline});
            _currentCall.State = CallState.Rejected;
        }

        private void GetRespond(IPort sender, Respond respond)
        {
            if (respond.State == RespondState.Accept)
            {
                AcceptRespond(sender, respond);
            }
            else DeclineRespond(sender, respond);
        }

        private void AcceptRespond(IPort sender, Respond respond)
        {
            CreateCall(sender.Terminal.Number, respond.Request.Source);
        }

        private void DeclineRespond(IPort sender, Respond respond)
        {
            const string message = "Call rejected";
            ((Port) sender).NewStationRespond(new StationRespond()
                {Request = respond.Request, DeclineMessage = message, State = RespondState.Decline});
            _currentCall.State = CallState.Rejected;
        }

        private void CreateCall(PhoneNumber source, PhoneNumber target)
        {
            _currentCall.State = CallState.Accept;
            if (source == target) throw new IncorrectNumberException("Incorrect Number");
            var sourcePort = GetPortByNumber(source);
            var targetPort = GetPortByNumber(target);
            const string message = "Call Started";
            sourcePort.NewStationRespond(new StationRespond(){Request = sourcePort.CurrentRequest, State = RespondState.Accept, AcceptMessage = message});
            targetPort.NewStationRespond(new StationRespond(){Request = sourcePort.CurrentRequest,State = RespondState.Accept, AcceptMessage = message});
            _currentCall.Start(DateTime.Now);
            sourcePort.CallEnd += ((sender, args) =>  EndCall((Port)sender,targetPort));
            targetPort.CallEnd += ((sender, args) =>  EndCall((Port)sender,sourcePort));
        }

        private void EndCall(Port source,Port target)
        {
            _currentCall.End(DateTime.Now);
            target.OnEndCall(source);
        }
    }
}