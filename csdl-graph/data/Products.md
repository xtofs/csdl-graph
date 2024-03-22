# csdl

```mermaid
graph
n0[: $ROOT]
n1[sales: Schema]
n2[Product: EntityType]
n3[id: Property]
n4[category: NavigationProperty]
n5[@Core.Description: Annotation]
n6[Category: EntityType]
n7[id: Property]
n8[name: Property]
n9[@Core.Description: Annotation]
n10[Edm: Schema]
n11[String: PrimitiveType]
n12[Int32: PrimitiveType]
n13[Boolean: PrimitiveType]
n14[Core: Schema]
n15[Description: Term]
n16[IsLanguageDependent: Term]
n17[Tag: TypeDefinition]
n18[RequiresType: Term]
n0-->n1
n0-->n10
n0-->n14
n1-. $contained .-> n0
n1-->n2
n1-->n6
n2-. $contained .-> n1
n2-->n3
n2-->n4
n3-. $contained .-> n2
n3-. Type .-> n11
n4-. $contained .-> n2
n4-->n5
n4-. Type .-> n6
n5-. $contained .-> n4
n5-. Term .-> n15
n6-. $contained .-> n1
n6-->n7
n6-->n8
n6-->n9
n7-. $contained .-> n6
n7-. Type .-> n11
n8-. $contained .-> n6
n8-. Type .-> n11
n9-. $contained .-> n6
n9-. Term .-> n15
n10-. $contained .-> n0
n10-->n11
n10-->n12
n10-->n13
n11-. $contained .-> n10
n12-. $contained .-> n10
n13-. $contained .-> n10
n14-. $contained .-> n0
n14-->n15
n14-->n16
n14-->n17
n14-->n18
n15-. $contained .-> n14
n15-. Type .-> n11
n16-. $contained .-> n14
n16-. Type .-> n17
n17-. $contained .-> n14
n17-. UnderlyingType .-> n13
n18-. $contained .-> n14
n18-. Type .-> n11
```
