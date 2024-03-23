# csdl

```mermaid
graph
n1[Edm: Schema]
n2[AnnotationPath: PrimitiveType]
n3[AnyPropertyPath: PrimitiveType]
n4[Binary: PrimitiveType]
n5[Boolean: PrimitiveType]
n6[Byte: PrimitiveType]
n7[ComplexType: PrimitiveType]
n8[Date: PrimitiveType]
n9[DateTimeOffset: PrimitiveType]
n10[Decimal: PrimitiveType]
n11[Double: PrimitiveType]
n12[Duration: PrimitiveType]
n13[EntityType: PrimitiveType]
n14[Geography: PrimitiveType]
n15[GeographyCollection: PrimitiveType]
n16[GeographyLineString: PrimitiveType]
n17[GeographyMultiLineString: PrimitiveType]
n18[GeographyMultiPoint: PrimitiveType]
n19[GeographyMultiPolygon: PrimitiveType]
n20[GeographyPoint: PrimitiveType]
n21[GeographyPolygon: PrimitiveType]
n22[Geometry: PrimitiveType]
n23[GeometryCollection: PrimitiveType]
n24[GeometryLineString: PrimitiveType]
n25[GeometryMultiLineString: PrimitiveType]
n26[GeometryMultiPoint: PrimitiveType]
n27[GeometryMultiPolygon: PrimitiveType]
n28[GeometryPoint: PrimitiveType]
n29[GeometryPolygon: PrimitiveType]
n30[Guid: PrimitiveType]
n31[Int: PrimitiveType]
n32[ModelElementPath: PrimitiveType]
n33[NavigationPropertyPath: PrimitiveType]
n34[PrimitiveType: PrimitiveType]
n35[PropertyPath: PrimitiveType]
n36[SByte: PrimitiveType]
n37[Single: PrimitiveType]
n38[Stream: PrimitiveType]
n39[String: PrimitiveType]
n40[TimeOfDay: PrimitiveType]
n41[Untyped: PrimitiveType]
n42[sales: Schema]
n43[Product: EntityType]
n44[`unnamed unknwon Property`: Property]
n45[`unnamed unknwon NavigationProperty`: NavigationProperty]
n46[@Core.Description: Annotation]
n47[Category: EntityType]
n48[`unnamed unknwon Property`: Property]
n49[`unnamed unknwon Property`: Property]
n50[@Core.Description: Annotation]
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
n1-->n26
n1-->n27
n1-->n28
n1-->n29
n1-->n30
n1-->n31
n1-->n32
n1-->n33
n1-->n34
n1-->n35
n1-->n36
n1-->n37
n1-->n38
n1-->n39
n1-->n40
n1-->n41
n42-->n43
n42-->n47
n43-->n44
n43-->n45
n44-. Type .-> n39
n45-->n46
n45-. Type .-> n47
n47-->n48
n47-->n49
n47-->n50
n48-. Type .-> n39
n49-. Type .-> n39
```
