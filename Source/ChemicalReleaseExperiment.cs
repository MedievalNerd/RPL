using System;

namespace CustomExperiment
{
    public class ChemicalReleaseExperiment : ModuleScienceExperiment
    {
        #region Variables & Fields

        [KSPField(isPersistant = false)]
        public String ExperimentChemical;

        [KSPField(isPersistant = false)]
        public String TargetBody;

        [KSPField(isPersistant = false)]
        public String TargetSituation;


        #endregion


        #region Methods

        // Ethernet's Methods for ExperimentSituations (Below this point)
        // Removed atmosphere check, to allow for flyingHigh/Low situations on non atmospheric bodies. Well that's the idea. Let's try it out. 
        // Success!

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
                {
                    return true;
                }

                if (TargetSituation != currentSituation)
                {
                    return false;
                }
            }

            if (experimentlocation == ExperimentSituations.InSpaceLow)
            {
                currentSituation = "InSpaceLow";
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

            if (experimentlocation == ExperimentSituations.InSpaceHigh)
            {
                currentSituation = "InSpaceHigh";
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

            return false;

        }

        // END Confirm Situation Method
        private bool CheckBody()
        {
            if (vessel.mainBody.name == TargetBody)
                return true;

            return false;
        }

        private bool CheckChemical()
        {
            PartResource Chemical = this.part.Resources.list.Find(
                delegate(PartResource pr)
                {
                    return pr.resourceName == ExperimentChemical;
                }
            );

            if (!(Chemical == null) && Chemical.amount == 0)
            {
                return true;
            }

            return false;
        }


        // Seperate Method for chemical release

        private bool ValidateExperiment()
        {
            if (CheckBody() == false)
            {
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment is only calibrated for {0}!", TargetBody), 3, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment is only calibrated for {0} situation!", TargetSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment requires to have dispersed the entire canister of {0}!", ExperimentChemical), 3, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }

            if (ConfirmLocation(GetScienceSituation(vessel)) == false)
            {
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment is only calibrated for {0} situation!", TargetSituation), 3, ScreenMessageStyle.UPPER_CENTER);
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment requires to have dispersed the entire canister of {0}!", ExperimentChemical), 3, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }

            if (CheckChemical() == false)
            {
                ScreenMessages.PostScreenMessage(String.Format("Warning! This Experiment requires to have dispersed the entire canister of {0}!", ExperimentChemical), 3, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }

            if (ConfirmLocation(GetScienceSituation(vessel)) && CheckBody() && CheckChemical())
            {
                ScreenMessages.PostScreenMessage(String.Format("Success! We've completed the requirements to complete this experiment!"), 3, ScreenMessageStyle.UPPER_CENTER);
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

                base.DeployAction(p);
            }
        }

        new public void DeployExperiment()
        {
            if (ValidateExperiment())
            {

                base.DeployExperiment();
            }
        }

        #endregion
    }
}