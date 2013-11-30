#pragma once

#include "Globals.h"

using namespace std;

template<class T>
struct Model
{
	vector<T> vertices;
	vector<int> indices;

	Model(){}
	Model(Model &&other);
	Model &operator=(Model &&other);

	Model(Model&);
	Model &operator=(Model&);
};

typedef Model<VertexType> SimpleModel;

template<class T>
Model<T>::Model(Model &&other)
	:vertices(move(other.vertices)), indices(move(other.indices))
{
}

template<class T>
Model<T> &Model<T>::operator=(Model &&other)
{
	if(this != &other)
	{
		vertices = move(other.vertices);
		indices = move(other.indices);
	}
	return *this;
}
