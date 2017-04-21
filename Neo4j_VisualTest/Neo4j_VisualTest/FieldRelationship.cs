using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4j_VisualTest
{
    class FieldRelationship:Relationship, IRelationshipAllowingSourceNode<Field>, IRelationshipAllowingTargetNode<Row>
    {
        public static readonly string TypeKey = "CONTAINS";
        public string flightNumber { get; set; }
        public FieldRelationship(NodeReference targetNode) : base(targetNode)
        {
        }
        public override string RelationshipTypeKey
        {
                get { return TypeKey; }
        }
    }
}
