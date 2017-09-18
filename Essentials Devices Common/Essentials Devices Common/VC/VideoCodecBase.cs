﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials.Devices.Common.VideoCodec
{
    public abstract class VideoCodecBase : Device, IRoutingSinkWithSwitching, IUsageTracking, IHasDialer, IHasSharing, ICodecAudio
    {
        #region IUsageTracking Members

        /// <summary>
        /// This object can be added by outside users of this class to provide usage tracking
        /// for various services
        /// </summary>
        public UsageTracking UsageTracker { get; set; }

        #endregion

        #region IRoutingInputs Members

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }

        #endregion

        public BoolFeedback InCallFeedback { get; private set; }
        public BoolFeedback IncomingCallFeedback { get; private set; }

        abstract protected Func<bool> InCallFeedbackFunc { get; }
        abstract protected Func<bool> IncomingCallFeedbackFunc { get; }
        abstract protected Func<bool> PrivacyModeFeedbackFunc { get; }
        abstract protected Func<int> VolumeLevelFeedbackFunc { get; }
        abstract protected Func<bool> MuteFeedbackFunc { get; }
        abstract protected Func<string> SharingSourceFeedbackFunc { get; }

        public VideoCodecBase(string key, string name)
            : base(key, name)
        {
            InCallFeedback = new BoolFeedback(InCallFeedbackFunc);
            IncomingCallFeedback = new BoolFeedback(IncomingCallFeedbackFunc);
            PrivacyModeIsOnFeedback = new BoolFeedback(PrivacyModeFeedbackFunc);
            VolumeLevelFeedback = new IntFeedback(VolumeLevelFeedbackFunc);
            MuteFeedback = new BoolFeedback(MuteFeedbackFunc);
            SharingSourceFeedback = new StringFeedback(SharingSourceFeedbackFunc);

            InputPorts = new RoutingPortCollection<RoutingInputPort>();

            InCallFeedback.OutputChange += new EventHandler<EventArgs>(InCallFeedback_OutputChange);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InCallFeedback_OutputChange(object sender, EventArgs e)
        {
            if (UsageTracker != null)
            {
                if (InCallFeedback.BoolValue)
                    UsageTracker.StartDeviceUsage();
                else
                    UsageTracker.EndDeviceUsage();
            }
        }
        #region IHasDialer Members

        public abstract void Dial(string s);
        public abstract void EndCall();
        public abstract void AcceptCall();
        public abstract void RejectCall();
        public abstract void SendDtmf(string s);

        #endregion

        public virtual List<Feedback> Feedbacks
        {
            get
            {
                return new List<Feedback>
				{
					InCallFeedback,
                    IncomingCallFeedback,
                    PrivacyModeIsOnFeedback,
                    SharingSourceFeedback
				};
            }
        }

        public abstract void ExecuteSwitch(object selector);

        #region ICodecAudio Members

        public abstract void PrivacyModeOn();
        public abstract void PrivacyModeOff();
        public abstract void PrivacyModeToggle();
        public BoolFeedback PrivacyModeIsOnFeedback { get; private set; }


        public BoolFeedback MuteFeedback { get; private set; }

        public abstract void MuteOff();

        public abstract void MuteOn();

        public abstract void SetVolume(ushort level);

        public IntFeedback VolumeLevelFeedback { get; private set; }

        public abstract void MuteToggle();

        public abstract void VolumeDown(bool pressRelease);


        public abstract void VolumeUp(bool pressRelease);

        #endregion

        #region IHasSharing Members

        public abstract void StartSharing();
        public abstract void StopSharing();

        public StringFeedback SharingSourceFeedback { get; private set; }

        #endregion


    }
}