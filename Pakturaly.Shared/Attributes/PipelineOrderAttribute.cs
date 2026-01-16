using System.Diagnostics.CodeAnalysis;

namespace Pakturaly.Shared.Attributes {
    [ExcludeFromCodeCoverage(Justification = "")]
    [AttributeUsage(AttributeTargets.Class)]
    public class PipelineOrderAttribute : Attribute {
        public short Order { get; }

        public PipelineOrderAttribute(short order) {
            Order = order;
        }
    }
}
