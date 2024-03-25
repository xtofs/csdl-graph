# csdl

```mermaid
graph
n1[Edm: Schema]
n2[Binary: PrimitiveType]
n3[Boolean: PrimitiveType]
n4[Byte: PrimitiveType]
n5[Date: PrimitiveType]
n6[DateTimeOffset: PrimitiveType]
n7[Decimal: PrimitiveType]
n8[Double: PrimitiveType]
n9[Duration: PrimitiveType]
n10[Guid: PrimitiveType]
n11[Int: PrimitiveType]
n12[SByte: PrimitiveType]
n13[Single: PrimitiveType]
n14[Stream: PrimitiveType]
n15[String: PrimitiveType]
n16[TimeOfDay: PrimitiveType]
n17[AnnotationPath: PrimitiveType]
n18[AnyPropertyPath: PrimitiveType]
n19[ModelElementPath: PrimitiveType]
n20[NavigationPropertyPath: PrimitiveType]
n21[PropertyPath: PrimitiveType]
n22[Untyped: PrimitiveType]
n23[PrimitiveType: PrimitiveType]
n24[ComplexType: PrimitiveType]
n25[EntityType: PrimitiveType]
n26[self: Schema]
n27[Employee: EntityType]
n28[`unnamed unknwon Property`: Property]
n29[@Core.Description#X: Annotation]
n30[`unnamed PropertyRef`: PropertyRef]
n31[ReportingLine: EntityType]
n32[`unnamed unknwon NavigationProperty`: NavigationProperty]
n33[@Core.Description: Annotation]
n34[`unnamed unknwon NavigationProperty`: NavigationProperty]
n35[Manager: PropertyRef]
n36[Subordinate: PropertyRef]
n1-->n2
n1-->n3
n1-->n4
n1-->n5
n1-->n6
n1-->n7
n1-->n8
n1-->n9
n1-->n10
n1-->n11
n1-->n12
n1-->n13
n1-->n14
n1-->n15
n1-->n16
n1-->n17
n1-->n18
n1-->n19
n1-->n20
n1-->n21
n1-->n22
n1-->n23
n1-->n24
n1-->n25
n26-->n27
n26-->n31
n27-->n28
n27-->n29
n27-->n30
n28-. Type .-> n15
n31-->n32
n31-->n34
n31-->n35
n31-->n36
n32-->n33
n32-. Type .-> n27
n34-. Type .-> n27
```
