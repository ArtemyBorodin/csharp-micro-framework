using System;

// этот атрибут вешается на параметры метода
// значения этого параметра бери из тела формы 
[AttributeUsage(AttributeTargets.Parameter)]
public class FormDataAttribute : Attribute
{

}