#include "PrecompiledHeader.h"
#include "Instancer.h"


Instancer::Instancer(SimpleModel &model, ColorInstancedShader &shader, size_t maxObjectCount)
:BaseInstancer(model, shader, maxObjectCount)
{
}


Instancer::~Instancer()
{
}
