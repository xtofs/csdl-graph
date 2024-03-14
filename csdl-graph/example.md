```mermaid
graph
n0[self: Schema]
n1[Employee: EntityType]
n2[Name: Property]
n3[ReportingLine: EntityType]
n4[ReportsTo: NavigationProperty]
n5[DirectReport: NavigationProperty]
n0-->n1
n1-->n2
n0-->n3
n3-->n4
n3-->n5
n4-. Type .-> n1
n5-. Type .-> n1
```
