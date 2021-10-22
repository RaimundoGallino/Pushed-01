using UnityEngine;
using static DreadZitoTools.Utils;

#region BasePushState
public class BasePushState
{
    [SerializeField] private string stateName;
    public string StateName => stateName;

    // Fuerzan't
    [System.NonSerialized] protected float strength;
    public float Strength => strength;
    protected float defaultStrength;
    protected float originalStrength;
    public float OriginalStrength => originalStrength;
    // =======================

    // Porcentages
    [SerializeField] protected Percentage percentages = new Percentage();
    // ========================

    // Duration
    [SerializeField] protected Durations durations = new Durations();
    public float Duration => durations.duration;
    public float StartedTime => durations.startTime;
    public float DurationFrames => durations.durationInFrames;
    // ========================

    // Color demostrativo u.u
    [SerializeField] private Color color;
    public Color Color => color;
    // =========================
    
    [System.NonSerialized] protected Vector3 forceDirection;
    public Vector3 ForceDirection => forceDirection;
    [System.NonSerialized] protected Vector3 forceVector;
    public Vector3 ForceVector => forceVector;
    public Vector3 SetForceVector {set => forceVector = value;}

    [SerializeField] private ForceMode forceMode;
    public ForceMode ForceMode => forceMode;

    // Replicate values from
    public BasePushState(BasePushState obj = null)
    {
        if (obj == null)
            return;

        this.stateName = obj.stateName;
        this.color = obj.color;
        this.strength = obj.strength;
        this.defaultStrength = obj.defaultStrength;
        this.percentages = new Percentage(obj.percentages);
        this.durations = new Durations(obj.durations);
        this.forceMode = obj.forceMode;
    }

    [System.Serializable]
    protected class Percentage
    {
        public string pushForcePercentage = "100%";
        private string defaultForcePercentage;
        public string pushForceLossPercentage = "20%";
        public string pushForceGainPercentage = "20%";

        public Percentage(Percentage obj = null)
        {
            if (obj == null)
                return;

            this.pushForcePercentage = obj.pushForcePercentage;
            this.defaultForcePercentage = obj.defaultForcePercentage;
            this.pushForceLossPercentage = obj.pushForceLossPercentage;
            this.pushForceGainPercentage = obj.pushForceGainPercentage;
        }

        public void InitializeMe() {
            defaultForcePercentage = pushForcePercentage;
        }

        public string LosePower()
        {
            int currentPercent = IntParsePercent(pushForcePercentage);
            int lossPercent = IntParsePercent(pushForceLossPercentage);
            int percentageResult = currentPercent - lossPercent;
            string newPersentage = "";

            if (percentageResult > 0)
                newPersentage = percentageResult.ToString() + '%';
            else
                pushForcePercentage = "0%";

            return newPersentage;
        }

        public string GainPower()
        {
            int currentPercent = IntParsePercent(pushForcePercentage);
            int gainPercent = IntParsePercent(pushForceGainPercentage);
            int defaultPercent = IntParsePercent(defaultForcePercentage);
            int percentageResult = currentPercent + gainPercent;
            string newPersentage = "";

            if (percentageResult >= defaultPercent)
                percentageResult = defaultPercent;

            newPersentage = percentageResult.ToString() + '%';
            pushForcePercentage = newPersentage;

            return newPersentage;
        }

        public int IntParsePercent(string perc) {
            return int.Parse(perc.Remove(perc.Length - 1));
        }
    }

    [System.Serializable]
    protected class Durations
    {
        public float duration;
        public float durationLoss = 0.02f;
        public float durationGain = 0.02f;
        private float defaultDuration;
        public float DurationLoss => durationLoss;

        [Tooltip("Duracion en frames que durará el estado cuando alcanza a un jugador enemigo")]
        public float durationInFrames;

        [System.NonSerialized] public float startTime;

        public Durations(Durations obj = null)
        {
            if (obj == null)
                return;

            this.duration = obj.duration;
            this.durationLoss = obj.durationLoss;
            this.durationGain = obj.durationGain;
            this.defaultDuration = obj.defaultDuration;
            this.durationInFrames = obj.durationInFrames;
        }

        public void InitializeMe() {
            defaultDuration = duration;
        }

        public void LosePower()
        {
            duration -= durationLoss;
            if (duration <= 0)
                duration = 0;
        }

        public void GainPower()
        {
            duration += durationGain;
            if (duration >= defaultDuration)
                duration = defaultDuration;
        }
    }

    public virtual void PrepareStateForSending(Vector3 forceDir)
    {
        this.forceDirection = forceDir.normalized;
        this.forceVector = this.forceDirection * this.Strength;
        this.durations.startTime = this.Duration + Time.time;
    }

    public virtual void LossPower()
    {
        durations.LosePower();

        var losedPercentage = percentages.LosePower();
        percentages.pushForcePercentage = losedPercentage;

        strength = GetPercentage(defaultStrength, losedPercentage);
    }

    public void GainPower()
    {
        durations.GainPower();
        strength = GetPercentage(defaultStrength, percentages.GainPower());
    }

    public override string ToString()
    {
        var str = $"{this.GetType().Name}:\n";
        str += $"Name: {this.StateName}\n";
        str += $"PushForcePercentage: {this.percentages.pushForcePercentage}\n";
        str += $"Strength: {strength}\n";
        str += $"Duration: {this.durations.duration}";
        str += "\n\n";

        return str;
    }
}
#endregion

#region StrengthPushState
[System.Serializable]
public class StrenghtPushState : BasePushState
{
    private float impulseForce;
    public float ImpulseForce => impulseForce;
    private float defaultImpulseForce;

    // Replicar desde otro objeto
    public StrenghtPushState(StrenghtPushState obj = null) : base(obj)
    {
        this.impulseForce = obj.impulseForce;
        this.defaultImpulseForce = obj.defaultImpulseForce;
    }

    public void InitializeState(float pushForce, float impulseForce)
    {
        percentages.InitializeMe();
        durations.InitializeMe();
        strength = GetPercentage(pushForce, percentages.pushForcePercentage);
        defaultStrength = strength;
        originalStrength = pushForce;
        this.impulseForce = impulseForce;
        this.defaultImpulseForce = impulseForce;
    }

    public override void LossPower()
    {
        base.LossPower();

        impulseForce = GetPercentage(defaultImpulseForce, percentages.pushForcePercentage);
    }

    public StrenghtPushState Clone()
    {
        return new StrenghtPushState(this);
    }
}
#endregion


[System.Serializable]
public class StrengthExplotionState : BasePushState
{
    public void InitializeState(float pushForce)
    {
        percentages.InitializeMe();
        durations.InitializeMe();
        strength = GetPercentage(pushForce, percentages.pushForcePercentage);
        defaultStrength = strength;
        originalStrength = pushForce;
    }
}
