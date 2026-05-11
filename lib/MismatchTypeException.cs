using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace No1.NHibernateNodaTime;

public class MismatchTypeException : Exception
{
	public MismatchTypeException()
	{
	}

	public MismatchTypeException(string? message) : base(message)
	{
	}

	public MismatchTypeException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}