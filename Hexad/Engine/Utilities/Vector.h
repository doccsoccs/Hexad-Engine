#pragma once

template<class T>
class Vector
{
	T* vector;
	u32 size;

public:
	// Appends data to the next NULL item of a vector, or end of one
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
			// If there is a NULL item in the array insert the data there and exit the function
			for (u32 i = 0; i < size; i++) 
			{
				if (vector[i] == NULL) vector[i] = data;
				return;
			}

			// Copy the existing vector array
			T* copyVector = new T[size + 1];
			for (u32 i = 0; i < size; i++) { copyVector[i] = vector[i]; }

			// Insert new data
			copyVector[size] = data;

			// Increment size
			size++;

			// Delete old vector array
			delete[] vector;

			// Fill vector array with copied data
			for (u32 i = 0; i < size; i++) { vector[i] = copyVector[i]; }

			// Clear the copied queue from the heap
			delete[] copyQueue;
			copyQueue = nullptr;
		}
	}

	// Nulls a piece of data from the vector
	void Remove(T data)
	{
		if (vector != nullptr) 
		{
			// Mark the first instance of the data found in the vector array as null
			for (u32 i; i < size; i++)
			{
				if (vector[i] == data) vector[i] = NULL;
				break;
			}
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
		for (u32 i = 0; i < size; i++)
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
			for (u32 i = 0; i < size; i++)
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