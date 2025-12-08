namespace SevenStrikeModules.XGraph
{
    using System;

    [Serializable]
    public class class_ActionAssetCaptureData
    {
        public string name;
        public string ver;
        public string datetime;
        public string data;

        public class_ActionAssetCaptureData Clone()
        {
            class_ActionAssetCaptureData data = new class_ActionAssetCaptureData();

            data.name = this.name;
            data.ver = this.ver;
            data.datetime = this.datetime;
            data.data = this.data;

            return data;
        }
    }
}