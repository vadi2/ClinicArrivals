﻿using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.Appointment;

namespace Test.Models
{
    // all Tests are set to happen on the arbitrary date of 1-1-21
    // the time of execution varies depending on the type of test that has being done
    [TestClass]
    public class MessageEngineTester
    {
        private readonly List<SmsMessage> OutputMsgs = new List<SmsMessage>();
        private readonly List<StorageOp> StorageOps = new List<StorageOp>();

        [TestMethod]
        public void testPreRegistrationMessageSent()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 9, 0, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            // set it up:
            nl.Add(tomorrowsAppointment());
            // run it
            engine.ProcessUpcomingAppointments(new List<PmsAppointment>(), nl);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+0411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Patient Test Patient #1 has an appointment with Dr Adam Ant at 09:15 AM on 2-Jan. 3 hours prior to the appointment, you will be sent a COVID-19 screening check to decide whether you should do a video consultation rather than seeing the doctor in person", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("save-appt", StorageOps[0].type);
            Assert.AreEqual("1234", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.PostRegistrationMessageSent);
        }

        [TestMethod]
        public void testPreRegistrationMessageNotSent()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 9, 0, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            nl.Add(tomorrowsAppointment());
            sl.Add(tomorrowsAppointment());
            // run it
            engine.ProcessUpcomingAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testPreRegistrationMessageNotSentForToday()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 9, 0, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            // run it
            engine.ProcessUpcomingAppointments(new List<PmsAppointment>(), nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testNothingToDo()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 10, 55, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt2pm());
            sl.Add(appt10am());
            sl.Add(appt2pm());
            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testScreeningMsg()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 10, 55, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+0411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligible to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ScreeningMessageSent);
        }

        [TestMethod]
        public void testScreeningMsgDone()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 10, 56, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ScreeningMessageSent = true;
            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testVideoInviteNotYet()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 45, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ScreeningMessageSent = true;
            sl[1].IsVideoConsultation = true;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testVideoInviteNow()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 51, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ScreeningMessageSent = true;
            nl[1].IsVideoConsultation = true;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+0411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Please start your video call at https://meet.jit.si/:guid:-1002. When you have started it, reply to this message with the word \"joined\"", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.VideoInviteSent);
        }

        // test case generation
        private PmsAppointment tomorrowsAppointment()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #1";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1234";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 2, 9, 15, 0);
            return app;
        }

        private PmsAppointment appt10am()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #1";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1000";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 1, 10, 0, 0);
            return app;
        }

        private PmsAppointment appt1pm()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #2";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1002";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 1, 13, 0, 0);
            return app;
        }

        private PmsAppointment appt2pm()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #3";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1002";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 1, 14, 0, 0);
            return app;
        }

        // supporting infrastructure
        private Settings testSettings()
        {
            return new Settings();
        }

        private MessagingEngine makeEngine()
        {
            MessagingEngine engine = new MessagingEngine();
            engine.Initialise(testSettings());
            engine.SmsSender = new MessageLogicTesterSmsHandler(this);
            engine.Storage = new MessageLogicTesterStorageHandler(this);
            engine.TemplateProcessor = new TemplateProcessor();
            engine.TemplateProcessor.Initialise(testSettings());
            engine.VideoManager = new MessageLogicTesterVideoHandler(this);
            loadTestTemplates(engine.TemplateProcessor);
            return engine;
        }

        private void loadTestTemplates(TemplateProcessor tp)
        {
            tp.Templates = new System.Collections.ObjectModel.ObservableCollection<MessageTemplate>();
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_REGISTRATION, "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. 3 hours prior to the appointment, you will be sent a COVID-19 screening check to decide whether you should do a video consultation rather than seeing the doctor in person"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING, "Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligible to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_VIDEO_INVITE, "Please start your video call at {{url}}. When you have started it, reply to this message with the word \"joined\""));
        }

        private void reset()
        {
            OutputMsgs.Clear();
            StorageOps.Clear();
        }

        private class MessageLogicTesterVideoHandler : IVideoConferenceManager
        {
            private MessageEngineTester owner;

            public MessageLogicTesterVideoHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }
            public void Initialize(Settings settings)
            {
            }

            public bool canKnowIfJoined()
            {
                return false;
            }

            public string getConferenceUrl(string appointmentId)
            {
                return "https://meet.jit.si/:guid:-" + appointmentId;
            }

            public bool hasSomeoneJoined(string appointmentId)
            {
                return false;
            }


        }

        private class MessageLogicTesterSmsHandler : ISmsProcessor
        {
            private MessageEngineTester owner;

            public MessageLogicTesterSmsHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }

            public void Initialize(Settings settings)
            {
            }

            public Task<IEnumerable<SmsMessage>> ReceiveMessages()
            {
                return null; // we don't actually use that
            }

            public void SendMessage(SmsMessage sendMessage)
            {
                owner.OutputMsgs.Add(sendMessage);
            }
        }

        private class MessageLogicTesterStorageHandler : IArrivalsLocalStorage
        {
            private MessageEngineTester owner;

            public MessageLogicTesterStorageHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }

            public Task CleanupHistoricCont()
            {
                throw new NotImplementedException();
            }

            public Task LoadAppointmentStatus(string date, PmsAppointment appt)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<DoctorRoomLabelMapping>> LoadRoomMappings()
            {
                throw new NotImplementedException();
            }

            public Task<Settings> LoadSettings()
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<MessageTemplate>> LoadTemplates()
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<SmsMessage>> LoadUnprocessableMessages(string date)
            {
                throw new NotImplementedException();
            }

            public Task SaveAppointmentStatus(string date, PmsAppointment appt)
            {
                owner.StorageOps.Add(new StorageOp("save-appt", appt));
                return null;
            }

            public Task SaveRoomMappings(IEnumerable<DoctorRoomLabelMapping> mappings)
            {
                throw new NotImplementedException();
            }

            public Task SaveSettings(Settings settings)
            {
                throw new NotImplementedException();
            }

            public Task SaveTemplates(IEnumerable<MessageTemplate> templates)
            {
                throw new NotImplementedException();
            }

            public Task SaveUnprocessableMessage(string date, SmsMessage message)
            {
                throw new NotImplementedException();
            }
        }
        private class StorageOp
        {

            public StorageOp(string type, PmsAppointment Appointment)
            {
                this.type = type;
                this.Appointment = Appointment;
            }

            public PmsAppointment Appointment { get; set; }
            public string type { get; set; }

        }
    }

}
