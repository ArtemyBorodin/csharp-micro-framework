using System;

// я создал метку (атрибут) по которой роутер сможет понимать что это контроллер
// его можно рассматривать при поиске метода    
[AttributeUsage(AttributeTargets.Class)]
public class HttpControllerAttribute : Attribute
{
    public string Name { get; }

    public HttpControllerAttribute(string name)
    {
        Name = name;
    }
}
