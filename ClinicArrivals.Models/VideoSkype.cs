﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public class VideoSkype : IVideoConferenceManager
    {
 
        public void Initialize(Settings settings)
        {
        }

        /// <summary>
        /// Get URL for conference
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public String getConferenceDetails(PmsAppointment appointment, Boolean GetItReady)
        {
            return null;
        }

        /// <summary>
        /// Return true if it's possible to know if the patient has joined (not always possible with video services)
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public Boolean canKnowIfJoined()
        {
            return false;
        }

        /// <summary>
        /// Return true if someone (assumed to be the patient) has joined the conference call
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public Boolean hasSomeoneJoined(String appointmentId)
        {
            return false;
        }

        public int getNotificationMinutes()
        {
            return 10;
        }


        public bool AsksForVideoUrl()
        {
            return true;
        }

    }
}
