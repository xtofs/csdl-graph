# CSDL Graph

[![build and test](https://github.com/xtofs/csdl-graph/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/xtofs/csdl-graph/actions/workflows/build-and-test.yml)

## Parsing CSDL into a model graph from a model specification

the "model" specification has 1 primary and 2 secondary parts

### meta model "schema"

The meta model schema is essentially a schema for a labeled property graph
(see [LPG](https://www.oxfordsemantic.tech/faqs/what-is-a-labeled-property-graph) )

Each CSDL Model Element is a Node in the graph with a label and

- a small set of named properties with a value of a primitive type (string, int, bool))
- a set of [contained](https://www.softwareideas.net/uml-class-diagram#containment) Model elements, each with a Label of a small set of alternatives
- a set of non-contained, associated ModelElements, each with a Label of a small set of alternatives.

### additional specification

In addition to the schema, it is necessary to specify how model paths are formed and how the individual elements are named in the path. This is done via functions XmlCsdlLoader.GetSeparator and XmlCsdlLoader.GetName

## Appendix

[Issue ODATA-1062](https://issues.oasis-open.org/browse/ODATA-1062?focusedCommentId=84136&page=com.atlassian.jira.plugin.system.issuetabpanels%3Acomment-tabpanel#comment-84136)


[mermaid cheat sheet](https://jojozhuang.github.io/tutorial/mermaid-cheat-sheet/)
