```mermaid
graph
0[Model]
1[Edm: Schema]
2[Core: Schema]
3[sales: Schema]
4[String: PrimitiveType]
5[Int32: PrimitiveType]
6[Description: Term]
7[Product: EntityType]
8[Category: EntityType]
9[id: Property]
10[category: NavigationProperty]
11[id: Property]
12[@Core.Description: Annotation]
13[@Core.Description: Annotation]
0-->1
0-->2
0-->3
1-->4
1-->5
2-->6
3-->7
3-->8
6-.Type.->4
7-->9
7-->10
8-->11
8-->12
9-.Type.->4
10-.Type.->8
10-->13
11-.Type.->4
12-.Term.->6
13-.Term.->6
```
