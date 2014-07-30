using System;
namespace CustomExperiment
{
    public class CustomExperiment : ModuleScienceExperiment
    {
        [KSPField(isPersistant = false)]
        public string ExperimentResource;
        [KSPField(isPersistant = false)]
        public float ExperimentCost;
        [KSPField(isPersistant = false)]
        public string TargetBody;
        [KSPField(isPersistant = false)]
        public string TargetSituation;
        private ExperimentSituations GetScienceSituation(Vessel vessel)
        {
            return GetScienceSituation(vessel.altitude, vessel.situation, vessel.mainBody);
        }
        public ExperimentSituations GetScienceSituation(double altitude, Vessel.Situations situation, CelestialBody body)
        {
            CelestialBodyScienceParams scienceValues = body.scienceValues;
            ExperimentSituations result;
            if (situation == Vessel.Situations.LANDED)
            {
                result = ExperimentSituations.SrfLanded;
            }
            else
            {
                if (situation == Vessel.Situations.SPLASHED)
                {
                    result = ExperimentSituations.SrfSplashed;
                }
                else
                {
                    if (altitude <= (double)scienceValues.flyingAltitudeThreshold)
                    {
                        result = ExperimentSituations.FlyingLow;
                    }
                    else
                    {
                        if (altitude < Math.Max((double)body.maxAtmosphereAltitude, body.atmosphereScaleHeight * 1000.0 * 13.8))
                        {
                            result = ExperimentSituations.FlyingHigh;
                        }
                        else
                        {
                            if (altitude <= (double)scienceValues.spaceAltitudeThreshold)
                            {
                                result = ExperimentSituations.InSpaceLow;
                            }
                            else
                            {
                                result = ExperimentSituations.InSpaceHigh;
                            }
                        }
                    }
                }
            }
            return result;
        }
        public bool ConfirmLocation(ExperimentSituations experimentlocation)
        {
            bool result;
            if (experimentlocation == ExperimentSituations.SrfLanded)
            {
                string text = "SrfLanded";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.SrfSplashed)
            {
                string text = "SrfSplashed";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.FlyingLow)
            {
                string text = "FlyingLow";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.FlyingHigh)
            {
                string text = "FlyingHigh";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.InSpaceLow)
            {
                string text = "InSpaceLow";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            if (experimentlocation == ExperimentSituations.InSpaceHigh)
            {
                string text = "InSpaceHigh";
                ScreenMessages.PostScreenMessage(string.Format("Current Situation :{0}", text), 3f, ScreenMessageStyle.UPPER_CENTER);
                if (TargetSituation == text)
                {
                    result = true;
                    return result;
                }
                if (TargetSituation != text)
                {
                    result = false;
                    return result;
                }
            }
            result = false;
            return result;
        }
        private bool CheckBody()
        {
            return base.vessel.mainBody.name == TargetBody;
        }
        private bool CheckData()
        {
            PartResource partResource = base.part.Resources.list.Find((PartResource pr) => pr.resourceName == ExperimentResource);
            bool result;
            if (!(partResource == null) && partResource.amount >= (double)ExperimentCost)
            {
                result = true;
            }
            else
            {
                ScreenMessages.PostScreenMessage(string.Format("You need at least {0} units of Data to perform this experiment!", ExperimentCost), 3f, ScreenMessageStyle.UPPER_CENTER);
                result = false;
            }
            return result;
        }
        private bool ValidateExperiment()
        {
            bool result;
            if (!CheckBody())
            {
                ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment is calibrated for {0} only!", TargetBody), 3f, ScreenMessageStyle.UPPER_CENTER);
                result = false;
            }
            else
            {
                if (!ConfirmLocation(GetScienceSituation(base.vessel)))
                {
                    ScreenMessages.PostScreenMessage(string.Format("Warning! This Experiment is calibrated for {0} only!", TargetSituation), 3f, ScreenMessageStyle.UPPER_CENTER);
                    result = false;
                }
                else
                {
                    result = (CheckData() && (ConfirmLocation(GetScienceSituation(base.vessel)) && CheckBody() && CheckData()));
                }
            }
            return result;
        }
        public new void DeployAction(KSPActionParam p)
        {
            if (ValidateExperiment())
            {
                base.part.RequestResource(ExperimentResource, ExperimentCost);
                base.DeployAction(p);
            }
        }
        public new void DeployExperiment()
        {
            if (ValidateExperiment())
            {
                base.part.RequestResource(ExperimentResource, ExperimentCost);
                base.DeployExperiment();
            }
        }
    }
}