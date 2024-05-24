#pragma once

template<class T>
class Vector
{
	T* vector;
	u32 size;

public:
	// Appends data to the end of a vector
	void Add(T data)
	{
		// Create an array for the vector if there isn't one
		if (vector == nullptr)
		{
			vector = new T[size];
			vector[0] = data;
		}

		else 
		{
			// Copy the existing vector array
			T* copyVector = new T[size];
			for (int i = 0; i < size; i++) { copyVector[i] = priorityQueue[i]; }

			// Clear the existing vector array without the new data
			size++;
			delete[] vector;
			vector = new T[size];

			// TODO AAAAAAs

			// Clear the copied queue from the heap
			delete[] copyQueue;
			copyQueue = nullptr;
		}
	}

	// Removes a piece of data from the vector
	void Remove(T data)
	{
		if (vector != nullptr) 
		{

		}
	}

	// Returns the number of elements in the vector array
	// Returns 'size' unless the vector is a nullptr with no elements
	u32 Count()
	{
		if (vector != nullptr)
		{
			return size;
		}
		return 0;
	}

	// Returns a true if there are any elements in the vector 
	bool IsEmpty()
	{
		if (Count() == 0) return true;
		else return false;
	}

	// Cleans the data in the vector from memory and sets it to a nullptr
	void Release()
	{
		size = 0;
		if (vector != nullptr)
		{
			delete[] vector;
			vector = nullptr;
		}
	}

// ------------- Vector constructions -------------
public:
	// CTOR
	Vector()
	{
		// initialize array and size tracker
		vector = nullptr;
		size = 1;
	}

	// Copy CTOR
	Vector(Vector& other)
	{
		size = other.size;

		vector = new T[size];
		for (int i = 0; i < size; i++)
		{
			vector[i] = other.vector[i];
		}
	}

	// Copy Assignment CTOR
	Vector& operator=(Vector& other) // Called when you set a vector equal to another vector
	{
		if (this != other) // if the inputed vector is different...
		{
			Release(); // release the current vector from memory

			size = other.size; // capture the new size

			// overwrite the vector array's data with the inputed vector's data
			vector = new T[size];
			for (int i = 0; i < size; i++)
			{
				vector[i] = other.vector[i];
			}
		}

		// return a pointer to this vector
		return *this;
	}
	
	// Destructor
	~Vector()
	{
		Release();
	}
};