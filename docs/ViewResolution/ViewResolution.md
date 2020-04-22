## View resolution

From the documentation's main page:

> Inferno targets LOB applications and requires knowledge of the frameworks it is build from. One can only get the most (out) of it after studying the main concepts and drives behind Caliburn-Micro and ReactiveUI. 

Instead of putting all the knowledge found in those repos' documentation in to my own words, I'd rather point out where Inferno differs. 

- Caliburn-Micro can be used both from the ViewModel-First and View-First perspective. Inferno, as ReactiveUI, can only be used with ViewModel-First.
- Inferno uses a limited portion of Caliburn-Micro's conventions system. Only the part used for view resolution is implemented. To be sure, there is no `Action` or `Property` matching in Inferno, instead ReactiveUI's binding system is used.

I encourage you to go through Caliburn-Micro's [documentation](https://caliburnmicro.com/documentation/) to study the relevant topics (if you haven't done so already), which include:

- [Screens, Conductors and Composition](https://caliburnmicro.com/documentation/composition)
- [All About Conventions](https://caliburnmicro.com/documentation/conventions)        
  - [View / ViewModel Naming Conventions](https://caliburnmicro.com/documentation/naming-conventions)
  - [Handling Custom Conventions](https://caliburnmicro.com/documentation/custom-conventions)
  - [Using the NameTransformer](https://caliburnmicro.com/documentation/name-transformer)

One of the difficulties with ViewModel - View communication is allowing for the views to be disposed of when they are no longer needed. Most frameworks, including Caliburn-Micro, use weak references to link the two, without keeping the view alive for longer than necessary. Instead of resorting to weak references, ReactiveUI retrieves a view sink for any view where the bound viewmodel implements a certain contract. The view can then execute the blocks of viewmodel code found in the sink, once it has been added to the visual tree. Inferno doesn't use weak references either and extends further upon the approach taken by ReactiveUI. This means a viewmodel is completely view agnostic. Separation of concerns at its best.



#### Next

It's important to be aware of the name clash between Caliburn-Micro and ReactiveUI when it comes to lifecycle events. A topic reserved for the first part of the documentation on component lifecycle: [Definition](../LifeCycle/1_Definition.md).

