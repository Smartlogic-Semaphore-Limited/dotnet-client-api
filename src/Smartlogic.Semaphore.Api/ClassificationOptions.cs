namespace Smartlogic.Semaphore.Api
{
    public class ClassificationOptions
    {
        /// <summary>
        ///     Gets or sets the threshold.
        /// </summary>
        /// <value>The threshold.</value>
        /// <remarks></remarks>
        public int? Threshold { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks></remarks>
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets the type of the clustering.
        /// </summary>
        /// <value>The type of the clustering.</value>
        /// <remarks></remarks>
        public string ClusteringType { get; set; }

        /// <summary>
        ///     Gets or sets the clustering threshold.
        /// </summary>
        /// <value>The clustering threshold.</value>
        /// <remarks></remarks>
        public int? ClusteringThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the type of the article.
        /// </summary>
        /// <value>The type of the article.</value>
        /// <remarks></remarks>
        public ArticleType ArticleType { get; set; }
    }
}