using System;

// метка для методов которые должны
// срабатывать на Get запрос.

[AttributeUsage(AttributeTargets.Method)]
public class GetAttribute : Attribute
{

}
