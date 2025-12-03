import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoInstalacionFormComponent } from './info-instalacion-form.component';

describe('InfoInstalacionFormComponent', () => {
  let component: InfoInstalacionFormComponent;
  let fixture: ComponentFixture<InfoInstalacionFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfoInstalacionFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfoInstalacionFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
