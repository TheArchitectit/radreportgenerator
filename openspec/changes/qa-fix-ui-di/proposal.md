# Proposal: Introduce DI for WPF services (QA-19 + open item core-interfaces-and-di partial)

## Intent
MainViewModel constructs parser, agent, and report generator directly. Comments admit "DI would be better". Blocks testing and provider swap.

## Approach
Add Microsoft.Extensions.DependencyInjection; App.xaml.cs builds ServiceProvider; MainViewModel takes services via ctor (or CommunityToolkit source generators). Keep parameterless ctor for designer with service locator fallback if needed.
