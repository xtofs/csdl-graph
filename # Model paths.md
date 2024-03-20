# Model paths

## path segments

- If a path segment is a simple identifier, it MUST be the **name of a child model element** of the preceding path part.
- If a path segment is a qualified name, the segment MUST be the **name of a type in scope**.
- if a path segment start with an at (@).
  - The at (@) character MUST be followed by a **qualified name of a term** in scope.
  - Or The at (@) character MUST be followed by a **qualified name name of a term followed by a # and a simple identifier** (the annotation qualifier ).
- if a path segments **starting with a navigation property**, then followed by an at (@) character, then followed by **the qualified name**.


