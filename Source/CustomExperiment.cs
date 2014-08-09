using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomExperiment
{
    public class CustomExperiment : ModuleScienceExperiment
    {
        #region Variables & Fields

        [KSPField(isPersistant = false)]
        public String ExperimentResource;

        [KSPField(isPersistant = false)]
        public float ExperimentCost;

        [KSPField(isPersistant = false)]
        public String TargetBody;

        [KSPField(isPersistant = false)]
        public String TargetSituation;

        #endregion


        #region Methods


        // Ethernet's Methods for ExperimentSituations (Below this point)

        private ExperimentSituations GetScienceSituation(Vessel vessel)
        {
            return GetScienceSituation(vessel.altitude, vessel.situation, vessel.mainBody);
        }

        public ExperimentSituations GetScienceSituation(double altitude, Vessel.Situations situation, CelestialBody body)
        {
            CelestialBodyScienceParams pars = body.scienceValues;
            if (situation == Vessel.Situations.LANDED)
                return ExperimentSituations.SrfLanded;
            else if (situation == Vessel.Situations.SPLASHED)
                return ExperimentSituations.SrfSplashed;
            else if (altitude <= pars.flyingAltitudeThreshold)
                return ExperimentSituations.FlyingLow;
            else if (altitude <= body.maxAtmosphereAltitude) // -ln(10^-6)
                return ExperimentSituations.FlyingHigh;
            else if (altitude <= pars.spaceAltitudeThreshold)
                return ExperimentSituations.InSpaceLow;
            else
                return ExperimentSituations.InSpaceHigh;
        }



        // Ethernet's Methods for ExperimentSituations (Above this point)

        // START Confirm Situation Method

        public bool ConfirmLocation(ExperimentSituations experimentlocation)
        {

            string currentSituation = "Placeholder";


            if (experimentlocation == ExperimentSituations.SrfLanded)
            {
                currentSituation = "SrfLanded";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);

                if (TargetSituation == currentSituation)
                {
                    return true;
                }

                if (TargetSituation != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.SrfSplashed)
            {
                currentSituation = "SrfSplashed";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);

                if (TargetSituation == currentSituation)
                {
                    return true;
                }

                if (TargetSituation != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.FlyingLow)
            {
                currentSituation = "FlyingLow";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == currentSituation)
                {
                    return true;
                }

                if (TargetSituation != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.FlyingHigh)
            {
                currentSituation = "FlyingHigh";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == currentSituation)

                    return true;

                if (TargetSituation != currentSituation)
                    return false;
            }

            if (experimentlocation == ExperimentSituations.InSpaceLow)
            {
                currentSituation = "InSpaceLow";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == currentSituation)

                    return true;

                if (TargetSituation != currentSituation)
                    return false;
            }

            if (experimentlocation == ExperimentSituations.InSpaceHigh)
            {
                currentSituation = "InSpaceHigh";
                ScreenMessages.PostScreenMessage(String.Format("Current Situation :{0}", currentSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == currentSituation)

                    return true;

                if (TargetSituation != currentSituation)
                    return false;
            }

            return false;

        }

        // END Confirm Situation Method
        private bool CheckBody()
        {
            if (vessel.mainBody.name == TargetBody)
                return true;

            return false;
        }

        private bool CheckData()
        {
            PartResource Data = this.part.Resources.list.Find(
                delegate(PartResource pr)
                {
                    return pr.resourceName == ExperimentResource;
                }
            );

            if (!(Data == null) && Data.amount >= ExperimentCost)

                return true;

            ScreenMessages.PostScreenMessage(String.Format("You need at least {0} units of Data to perform this experiment!", ExperimentCost), 3, ScreenMessageStyle.UPPER_CENTER);
            return false;
        }

        private bool ValidateExperiment()
        {


            // return (CheckBody() && CheckData());
            if (CheckBody() == false)
            {
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment is calibrated for {0} only!", TargetBody), 3, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }


            if (ConfirmLocation(GetScienceSituation(vessel)) == false)
            {
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment is calibrated for {0} only!", TargetSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }

            if (CheckData() == false)
            {

                return false;
            }


            if (ConfirmLocation(GetScienceSituation(vessel)) && CheckBody() && CheckData())
            {
                return true;
            }

            return false;

        }

        #endregion

        #region KSP Actions

        new public void DeployAction(KSPActionParam p)
        {
            if (ValidateExperiment())
            {
                this.part.RequestResource(ExperimentResource, ExperimentCost);
                base.DeployAction(p);
            }
        }

        new public void DeployExperiment()
        {
            if (ValidateExperiment())
            {
                this.part.RequestResource(ExperimentResource, ExperimentCost);
                base.DeployExperiment();
            }
        }

        #endregion
    }
}