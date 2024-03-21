# csdl

```mermaid
graph
n0[: $ROOT]
n1[self: Schema]
n2[Employee: EntityType]
n3[Name: Property]
n4[@Core.Description#X: Annotation]
n5[Name: PropertyRef]
n6[ReportingLine: EntityType]
n7[ReportsTo: NavigationProperty]
n8[@Core.Description: Annotation]
n9[DirectReport: NavigationProperty]
n10[Manager: PropertyRef]
n11[Subordinate: PropertyRef]
n12[Edm: Schema]
n13[String: PrimitiveType]
n14[Int32: PrimitiveType]
n15[Boolean: PrimitiveType]
n16[Core: Schema]
n17[Description: Term]
n18[@Core.Description: Annotation]
n19[@Core.IsLanguageDependent: Annotation]
n20[IsLanguageDependent: Term]
n21[@Core.Description: Annotation]
n22[@Core.RequiresType: Annotation]
n23[Tag: TypeDefinition]
n24[@Core.Description: Annotation]
n25[RequiresType: Term]
n26[@Core.Description: Annotation]
n0-->n1
n0-->n12
n0-->n16
n1-->n2
n1-->n6
n2-->n3
n2-->n4
n2-->n5
n3-. Type .-> n13
n4-. Term .-> n17
n6-->n7
n6-->n9
n6-->n10
n6-->n11
n7-->n8
n7-. Type .-> n2
n8-. Term .-> n17
n9-. Type .-> n2
n12-->n13
n12-->n14
n12-->n15
n16-->n17
n16-->n20
n16-->n23
n16-->n25
n17-->n18
n17-->n19
n17-. Type .-> n13
n18-. Term .-> n17
n19-. Term .-> n20
n20-->n21
n20-->n22
n20-. Type .-> n23
n21-. Term .-> n17
n22-. Term .-> n25
n23-->n24
n23-. UnderlyingType .-> n15
n24-. Term .-> n17
n25-->n26
n25-. Type .-> n13
n26-. Term .-> n17
```
