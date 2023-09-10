namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 
    /// </summary>
    public class AdCallbackInfo
    {
        /// <summary>
        /// Gets or sets the ad unit identifier.
        /// </summary>
        /// <value>
        /// The ad unit identifier.
        /// </value>
        public string AdUnitId { get; set; }
        /// <summary>
        /// Gets or sets the name of the network.
        /// </summary>
        /// <value>
        /// The name of the network.
        /// </value>
        public string NetworkName { get; set; }
        /// <summary>
        /// Gets or sets the network placement.
        /// </summary>
        /// <value>
        /// The network placement.
        /// </value>
        public string NetworkPlacement { get; set; }
        /// <summary>
        /// Gets or sets the placement.
        /// </summary>
        /// <value>
        /// The placement.
        /// </value>
        public string Placement { get; set; }
        /// <summary>
        /// Gets or sets the revenue.
        /// </summary>
        /// <value>
        /// The revenue.
        /// </value>
        public double Revenue { get; set; }
    }
}
