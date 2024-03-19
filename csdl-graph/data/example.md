# csdl

```mermaid
graph
n0[: $ROOT]
n1[self: Schema]
n2[Employee: EntityType]
n3[Name: Property]
n4[@Core.Description#X: Annotation]
n5[ReportingLine: EntityType]
n6[ReportsTo: NavigationProperty]
n7[DirectReport: NavigationProperty]
n8[Edm: Schema]
n9[String: PrimitiveType]
n10[Int32: PrimitiveType]
n11[Boolean: PrimitiveType]
n12[Core: Schema]
n13[Description: Term]
n14[@Core.Description: Annotation]
n15[@Core.IsLanguageDependent: Annotation]
n16[IsLanguageDependent: Term]
n17[@Core.Description: Annotation]
n18[@Core.RequiresType: Annotation]
n19[Tag: TypeDefinition]
n20[@Core.Description: Annotation]
n21[RequiresType: Term]
n22[@Core.Description: Annotation]
n0-->n1
n0-->n8
n0-->n12
n1-->n2
n1-->n5
n2-->n3
n2-->n4
n3-. Type .-> n9
n4-. Term .-> n13
n5-->n6
n5-->n7
n6-. Type .-> n2
n7-. Type .-> n2
n8-->n9
n8-->n10
n8-->n11
n12-->n13
n12-->n16
n12-->n19
n12-->n21
n13-->n14
n13-->n15
n13-. Type .-> n9
n14-. Term .-> n13
n15-. Term .-> n16
n16-->n17
n16-->n18
n16-. Type .-> n19
n17-. Term .-> n13
n18-. Term .-> n21
n19-->n20
n19-. UnderlyingType .-> n11
n20-. Term .-> n13
n21-->n22
n21-. Type .-> n9
n22-. Term .-> n13
```
