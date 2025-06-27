# Plugins

Plugins allow you to extend the functionality of the validation framework.
You can create your own formatters, validation rules, or completions.
Plugins are loaded into the validation framework and used by the Validation Service or a validation application.

## Developing Plugins

You develop plugins for the validation framework in C# .NET-Standard 2.0.
Samples for custom validation rules are provided in the source code of this repository.
For information on using and configuring custom plugins in for the Validation Framework, please refer to the user guide shipped with the Validation Service.

## First Steps

The provided samples show the basics of how to implement custom rules.
For simple rules, you only need to inherit a base class and reference the following two assemblies:

- Skilja.Romeo.Validation.dll
- Skilja.Romeo.Validation.Interfaces.dll

Please make sure that your plugin assembly starts with `Skilja.Romeo.Validation.Plugins`, otherwise there may be cases where the plugin is not loaded correctly by the framework.

The base classes you need to inherit are:

- `ValidationBase`: or Singlefield or Multifield rules
- `BaseFormatter`: for Formatters
- `CompletionBase`: for Completions

The samples show how to acces the *Fields*, *Tables*, *Rows* and *Cells* of a document, which are called **Validatables** in the context of the Validation Framework.
We advise you to stick to the provided patterns to manipulate document data.
Otherwise the changed values are not handled properly in the validation cycle of the framework.

There is also a sample on how to use lookups in your custom rules.

>**Warning** The plugins provide the possibility to implement powerful business logic.
But we strongly advise you to stick to the given patterns and concepts.
Otherwise your plugins can have unexpected and hard to track down errors in the validation applications using the plugin.

## Supported Functionality

Plugins allow you to leverage the full power of the C# .NET environment, but we only support the following use cases:

- Functionality within the limits of existing concepts, as there are:  
    - **Formatter**. To set the typed value and the text value based on the given text value of a single field or cell.  
    - **Validation rules**. That are responsible for setting fields to *valid* or *invalid*.  
    - **Completions**. To set field values based on the underlying validation rule.  
- Database access is not supported, you can use the Lookup Service and plugin functionality there.  
- Simple and preferably fast functions.

The intended use of plugins is to provide custom calculations not available in the validation framework to implement business logic, such as custom row calculations or special formatting of values based on user input.
It is not intended to allow large time consuming calculations, as they do not make sense in the context of validation applications and can easily break products.
For complex business logic and checks, we recommend using a custom activity where you have full control over every aspect.

## Considerations

Please keep the following in mind when developing your own plugin:

- Make sure that there really is no existing rule that can handle your requirements.  
- Avoid large dependencies on other assemblies.  
For the intended use cases, a plugin should not require many other assemblies to run.  
- You do not need to provide the two dependencies (`Skilja.Romeo.Validation.dll`, `Skilja.Romeo.Validation.Interfaces.dll`) because they are always loaded by the application.  
- You also do not need to provide shared or system assemblies if they are already used by the Validation Framework.

## Important Notes

The following is easy to overlook when developing plugins:

- Plugins are loaded differently by different applications, depending on the technology of the hosting application.

    1. The Validation Service loads plugins in an ``AssemblyLoadContext``.  
    1. Document Review and Autovalidation load plugins into the application's ``AppDomain``.  
    1. When used in a *WebAssembly* context, plugins are loaded into the browser's sandbox, which limits some functionality such as file system access.  
    1. When using Docker, the Validation Service runs in Linux.

- Please make sure you test the plugin and all applications you want to use the plugin with.
Usually plugins work in all environments for the intended use cases.  
- In case of application upgrades, please check for changes in your used dependencies and test your plugins in the new application versions before using them in production.
Changes to the validation framework are usually documented in the release notes, but third-party dependencies may also change.  
- Be aware of potential memory leaks and avoid using static or temporary data storage, as managing the lifetime of such data, along with concurrency and race conditions, can be challenging.
Validation functions are designed to be instantiated and used once.